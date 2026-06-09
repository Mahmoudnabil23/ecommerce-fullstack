using E_Commerce_Backend.DTOs.Cart.Requests;
using E_Commerce_Backend.DTOs.Cart.Responses;
using E_Commerce_Backend.Models;
using CartModel = E_Commerce_Backend.Models.Cart;
using E_Commerce_Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce_Backend.Services.Cart
{
    public class CartService
    {
        private readonly ICartRepository _cartRepo;
        private readonly ICartItemRepository _cartItemRepo;
        private readonly IProductRepository _productRepo;
        private readonly AppDbContext _context;

        public CartService(
            ICartRepository cartRepository,
            ICartItemRepository cartItemRepository,
            IProductRepository productRepository,
            AppDbContext context)
        {
            _cartRepo = cartRepository;
            _cartItemRepo = cartItemRepository;
            _productRepo = productRepository;
            _context = context;
        }

        // Get cart for authenticated user or guest session
        public async Task<CartResponseDto> GetCartAsync(Guid? userId, string? guestSessionId)
        {
            CartModel? cart = null;

            if (userId.HasValue)
            {
                cart = await _cartRepo.GetByUserIdAsync(userId.Value);
            }
            else if (!string.IsNullOrEmpty(guestSessionId))
            {
                cart = await _cartRepo.GetByGuestSessionAsync(guestSessionId);
            }

            if (cart is null)
                return new CartResponseDto();

            var dto = new CartResponseDto
            {
                CartId = cart.Id,
                Items = cart.Items.Select(i => new CartItemDto
                {
                    CartItemId = i.Id,
                    Quantity = i.Quantity,
                    Product = new CartProductRefDto
                    {
                        Id = i.Product.Id,
                        Name = i.Product.Name,
                        Price = i.Product.Price,
                        ImageUrls = i.Product.Images.Select(i => i.ImageUrl).ToList()
                    },
                    Subtotal = i.Quantity * i.Product.Price
                }).ToList()
            };

            dto.Summary.Subtotal = dto.Items.Sum(x => x.Subtotal);
            dto.Summary.Shipping = 0;
            dto.Summary.Discount = 0;
            dto.Summary.Total = dto.Summary.Subtotal - dto.Summary.Discount + dto.Summary.Shipping;

            return dto;
        }

        public async Task<Guid> EnsureCartForUserAsync(Guid? userId, string? guestSessionId)
        {
            CartModel? cart = null;

            if (userId.HasValue)
            {
                cart = await _cartRepo.GetByUserIdAsync(userId.Value);
                if (cart is null)
                {
                    cart = new CartModel { UserId = userId.Value.ToString() };
                    await _cartRepo.AddAsync(cart);
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                if (string.IsNullOrEmpty(guestSessionId))
                {
                    cart = new CartModel();
                    await _cartRepo.AddAsync(cart);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    cart = await _cartRepo.GetByGuestSessionAsync(guestSessionId);
                    if (cart is null)
                    {
                        cart = new CartModel { GuestSessionId = guestSessionId };
                        await _cartRepo.AddAsync(cart);
                        await _context.SaveChangesAsync();
                    }
                }
            }

            return cart.Id;
        }

        public async Task<Guid?> AddItemAsync(Guid cartId, AddCartItemRequestDto dto)
        {
            var product = await _productRepo.GetByIdAsync(dto.ProductId);
            if (product is null)
                return null;

            var existing = await _cartItemRepo.GetByCartAndProductAsync(cartId, dto.ProductId);
            if (existing is not null)
            {
                existing.Quantity += dto.Quantity;
                _cartItemRepo.Update(existing);
                await _context.SaveChangesAsync();
                return existing.Id;
            }

            var item = new CartItem
            {
                CartId = cartId,
                ProductId = dto.ProductId,
                Quantity = dto.Quantity
            };

            await _cartItemRepo.AddAsync(item);
            await _context.SaveChangesAsync();

            return item.Id;
        }

        public async Task<bool> UpdateItemAsync(Guid cartId, Guid cartItemId, UpdateCartItemRequestDto dto)
        {
            var item = await _cartItemRepo.GetByIdAndCartAsync(cartItemId, cartId);
            if (item is null)
                return false;

            item.Quantity = dto.Quantity;
            _cartItemRepo.Update(item);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveItemAsync(Guid cartId, Guid cartItemId)
        {
            var item = await _cartItemRepo.GetByIdAndCartAsync(cartItemId, cartId);
            if (item is null)
                return false;

            _cartItemRepo.Delete(item);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MergeCartAsync(Guid targetCartId, string guestSessionId)
        {
            var guestCart = await _cartRepo.GetByGuestSessionWithItemsAsync(guestSessionId);
            if (guestCart is null)
                return false;

            var target = await _cartRepo.GetByIdAsync(targetCartId);
            if (target is null)
                return false;

            foreach (var item in guestCart.Items)
            {
                var existing = await _cartItemRepo.GetByCartAndProductAsync(target.Id, item.ProductId);
                if (existing is not null)
                {
                    existing.Quantity += item.Quantity;
                    _cartItemRepo.Update(existing);
                }
                else
                {
                    item.CartId = target.Id;
                    _cartItemRepo.Update(item);
                }
            }

            // delete guest cart
            _cartRepo.Delete(guestCart);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
