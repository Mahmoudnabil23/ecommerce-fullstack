using E_Commerce_Backend.Comman;
using E_Commerce_Backend.DTOs.Cart.Requests;
using E_Commerce_Backend.DTOs.Cart.Responses;
using E_Commerce_Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace E_Commerce_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CartController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<CartResponseDto>>> GetCart()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(new ApiResponse<CartResponseDto>
                {
                    Success = false,
                    Message = "Unauthorized"
                });
            }

            var cart = await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(ci => ci.Product)
                .ThenInclude(p => p.Images)
                .Include(c => c.AppliedPromoCode)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                return Ok(new ApiResponse<CartResponseDto>
                {
                    Success = true,
                    Data = new CartResponseDto
                    {
                        CartId = Guid.Empty,
                        Items = new List<CartItemDto>(),
                        Summary = new CartSummaryDto
                        {
                            Subtotal = 0,
                            Discount = 0,
                            Shipping = 0,
                            Total = 0
                        }
                    }
                });
            }

            var itemDtos = cart.Items.Select(ci => new CartItemDto
            {
                CartItemId = ci.Id,
                Quantity = ci.Quantity,
                Subtotal = ci.Quantity * ci.Product.Price,
                Product = new CartProductRefDto
                {
                    Id = ci.ProductId,
                    Name = ci.Product.Name,
                    Price = ci.Product.Price,
                    ImageUrls = ci.Product.Images
                        .OrderBy(i => i.DisplayOrder)
                        .Select(i => i.ImageUrl)
                        .ToList()
                }
            }).ToList();

            var subtotal = itemDtos.Sum(i => i.Subtotal);
            var discount = 0m;
            var shipping = subtotal > 0 ? 50m : 0m;
            var total = subtotal - discount + shipping;

            var cartDto = new CartResponseDto
            {
                CartId = cart.Id,
                Items = itemDtos,
                AppliedPromoCode = cart.AppliedPromoCode?.Code,
                Summary = new CartSummaryDto
                {
                    Subtotal = subtotal,
                    Discount = discount,
                    Shipping = shipping,
                    Total = total
                }
            };

            return Ok(new ApiResponse<CartResponseDto>
            {
                Success = true,
                Data = cartDto
            });
        }

        [HttpPost("items")]
        public async Task<ActionResult<ApiResponse>> AddItem([FromBody] AddCartItemRequestDto request)
        {
            if (request.Quantity <= 0)
            {
                return BadRequest(new ApiResponse { Success = false, Message = "Quantity must be greater than zero." });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(new ApiResponse { Success = false, Message = "Unauthorized" });
            }

            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == request.ProductId);
            if (product == null)
            {
                return NotFound(new ApiResponse { Success = false, Message = "Product not found." });
            }

            if (product.Stock < request.Quantity)
            {
                return BadRequest(new ApiResponse { Success = false, Message = "Requested quantity exceeds stock." });
            }

            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            var existing = cart.Items.FirstOrDefault(i => i.ProductId == request.ProductId);
            if (existing == null)
            {
                _context.CartItems.Add(new CartItem
                {
                    CartId = cart.Id,
                    ProductId = request.ProductId,
                    Quantity = request.Quantity
                });
            }
            else
            {
                var newQty = existing.Quantity + request.Quantity;
                if (product.Stock < newQty)
                {
                    return BadRequest(new ApiResponse { Success = false, Message = "Requested quantity exceeds stock." });
                }

                existing.Quantity = newQty;
                _context.CartItems.Update(existing);
            }

            cart.UpdatedAt = DateTime.UtcNow;
            _context.Carts.Update(cart);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse { Success = true, Message = "Item added to cart." });
        }

        [HttpPut("items/{cartItemId:guid}")]
        public async Task<ActionResult<ApiResponse>> UpdateItem(Guid cartItemId, [FromBody] UpdateCartItemRequestDto request)
        {
            if (request.Quantity <= 0)
            {
                return BadRequest(new ApiResponse { Success = false, Message = "Quantity must be greater than zero." });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(new ApiResponse { Success = false, Message = "Unauthorized" });
            }

            var item = await _context.CartItems
                .Include(i => i.Product)
                .Include(i => i.Cart)
                .FirstOrDefaultAsync(i => i.Id == cartItemId && i.Cart.UserId == userId);

            if (item == null)
            {
                return NotFound(new ApiResponse { Success = false, Message = "Cart item not found." });
            }

            if (item.Product.Stock < request.Quantity)
            {
                return BadRequest(new ApiResponse { Success = false, Message = "Requested quantity exceeds stock." });
            }

            item.Quantity = request.Quantity;
            item.Cart.UpdatedAt = DateTime.UtcNow;
            _context.CartItems.Update(item);
            _context.Carts.Update(item.Cart);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse { Success = true, Message = "Cart item updated." });
        }

        [HttpDelete("items/{cartItemId:guid}")]
        public async Task<ActionResult<ApiResponse>> RemoveItem(Guid cartItemId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(new ApiResponse { Success = false, Message = "Unauthorized" });
            }

            var item = await _context.CartItems
                .Include(i => i.Cart)
                .FirstOrDefaultAsync(i => i.Id == cartItemId && i.Cart.UserId == userId);

            if (item == null)
            {
                return NotFound(new ApiResponse { Success = false, Message = "Cart item not found." });
            }

            item.Cart.UpdatedAt = DateTime.UtcNow;
            _context.CartItems.Remove(item);
            _context.Carts.Update(item.Cart);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse { Success = true, Message = "Cart item removed." });
        }
    }
}