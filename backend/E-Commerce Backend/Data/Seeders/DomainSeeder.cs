using E_Commerce_Backend.Enums;
using E_Commerce_Backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce_Backend.Data.Seeders
{
    public class DomainSeeder
    {
        private const string SeedPassword = "Seed@12345";
        private const string SellerEmail = "seller.seed@site.com";
        private const string CustomerEmail = "customer.seed@site.com";

        private static readonly Guid ElectronicsCategoryId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        private static readonly Guid AccessoriesCategoryId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        private static readonly Guid PromoCodeId = Guid.Parse("33333333-3333-3333-3333-333333333333");
        private static readonly Guid SellerProfileId = Guid.Parse("44444444-4444-4444-4444-444444444444");

        private static readonly Guid MouseProductId = Guid.Parse("55555555-5555-5555-5555-555555555555");
        private static readonly Guid KeyboardProductId = Guid.Parse("66666666-6666-6666-6666-666666666666");

        private static readonly Guid MouseImageId = Guid.Parse("77777777-7777-7777-7777-777777777777");
        private static readonly Guid KeyboardImageId = Guid.Parse("88888888-8888-8888-8888-888888888888");

        private static readonly Guid CustomerAddressId = Guid.Parse("99999999-9999-9999-9999-999999999999");
        private static readonly Guid CustomerCartId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        private static readonly Guid CustomerCartItemId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

        private static readonly Guid StripePendingOrderId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");
        private static readonly Guid StripePendingOrderItemId = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd");
        private static readonly Guid StripePendingStatusHistoryId = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee");

        private static readonly Guid PaidOrderId = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff");
        private static readonly Guid PaidOrderItemId = Guid.Parse("12121212-1212-1212-1212-121212121212");
        private static readonly Guid PaidStatusHistoryId = Guid.Parse("13131313-1313-1313-1313-131313131313");

        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DomainSeeder(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task SeedAsync()
        {
            var sellerUser = await EnsureUserAsync(SellerEmail, "Seed Seller", "+201001112223", "Seller");
            var customerUser = await EnsureUserAsync(CustomerEmail, "Seed Customer", "+201009998887", "Customer");

            await EnsureSellerProfileAsync(sellerUser.Id);
            await EnsureCategoriesAsync();
            await EnsurePromoCodeAsync();
            await EnsureProductsAsync();
            await EnsureProductImagesAsync();
            await EnsureCustomerAddressAsync(customerUser.Id);
            await EnsureCustomerCartAsync(customerUser.Id);
            await EnsureOrdersAsync(customerUser.Id);

            await _context.SaveChangesAsync();
        }

        private async Task<ApplicationUser> EnsureUserAsync(string email, string fullName, string phone, string role)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    FullName = fullName,
                    PhoneNumber = phone,
                    EmailConfirmed = true,
                    Status = UserStatus.Active
                };

                var createResult = await _userManager.CreateAsync(user, SeedPassword);
                if (!createResult.Succeeded)
                {
                    var errorText = string.Join("; ", createResult.Errors.Select(e => e.Description));
                    throw new Exception($"Failed to create seed user {email}: {errorText}");
                }
            }

            var inRole = await _userManager.IsInRoleAsync(user, role);
            if (!inRole)
            {
                await _userManager.AddToRoleAsync(user, role);
            }

            return user;
        }

        private async Task EnsureSellerProfileAsync(string sellerUserId)
        {
            var sellerProfile = await _context.SellerProfiles
                .FirstOrDefaultAsync(s => s.Id == SellerProfileId || s.UserId == sellerUserId);

            if (sellerProfile == null)
            {
                sellerProfile = new SellerProfile
                {
                    Id = SellerProfileId,
                    UserId = sellerUserId,
                    StoreName = "Seed Tech Hub",
                    StoreDescription = "Seeded seller account for team testing.",
                    TaxId = "SEED-TAX-001",
                    Status = SellerStatus.Approved,
                    AverageRating = 4.5m,
                    BankName = "CIB",
                    BankAccountNumber = "000123456789",
                    BankAccountHolderName = "Seed Seller"
                };

                await _context.SellerProfiles.AddAsync(sellerProfile);
                return;
            }

            sellerProfile.StoreName = "Seed Tech Hub";
            sellerProfile.StoreDescription = "Seeded seller account for team testing.";
            sellerProfile.TaxId = "SEED-TAX-001";
            sellerProfile.Status = SellerStatus.Approved;
            sellerProfile.AverageRating = 4.5m;
            sellerProfile.BankName = "CIB";
            sellerProfile.BankAccountNumber = "000123456789";
            sellerProfile.BankAccountHolderName = "Seed Seller";
            sellerProfile.UserId = sellerUserId;
        }

        private async Task EnsureCategoriesAsync()
        {
            var electronics = await _context.Categories
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(c => c.Id == ElectronicsCategoryId || c.Slug == "electronics");

            if (electronics == null)
            {
                electronics = new Category
                {
                    Id = ElectronicsCategoryId,
                    Name = "Electronics",
                    Slug = "electronics",
                    ImageUrl = "https://picsum.photos/seed/electronics/800/450",
                    IsDeleted = false
                };
                await _context.Categories.AddAsync(electronics);
            }
            else
            {
                electronics.Name = "Electronics";
                electronics.Slug = "electronics";
                electronics.ImageUrl = "https://picsum.photos/seed/electronics/800/450";
                electronics.IsDeleted = false;
            }

            var accessories = await _context.Categories
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(c => c.Id == AccessoriesCategoryId || c.Slug == "electronics-accessories");

            if (accessories == null)
            {
                accessories = new Category
                {
                    Id = AccessoriesCategoryId,
                    Name = "Electronics Accessories",
                    Slug = "electronics-accessories",
                    ParentId = ElectronicsCategoryId,
                    ImageUrl = "https://picsum.photos/seed/accessories/800/450",
                    IsDeleted = false
                };
                await _context.Categories.AddAsync(accessories);
            }
            else
            {
                accessories.Name = "Electronics Accessories";
                accessories.Slug = "electronics-accessories";
                accessories.ParentId = ElectronicsCategoryId;
                accessories.ImageUrl = "https://picsum.photos/seed/accessories/800/450";
                accessories.IsDeleted = false;
            }
        }

        private async Task EnsurePromoCodeAsync()
        {
            var promoCode = await _context.PromoCodes
                .FirstOrDefaultAsync(p => p.Id == PromoCodeId || p.Code == "SEED10");

            if (promoCode == null)
            {
                promoCode = new PromoCode
                {
                    Id = PromoCodeId,
                    Code = "SEED10",
                    Type = PromoCodeType.Percentage,
                    Value = 10m,
                    MinOrderAmount = 100m,
                    UsageLimit = 1000,
                    UsageCount = 0,
                    ExpiresAt = DateTime.UtcNow.AddYears(1),
                    ApplicableCategoriesJson = $"[\"{ElectronicsCategoryId}\"]",
                    IsActive = true
                };

                await _context.PromoCodes.AddAsync(promoCode);
                return;
            }

            promoCode.Code = "SEED10";
            promoCode.Type = PromoCodeType.Percentage;
            promoCode.Value = 10m;
            promoCode.MinOrderAmount = 100m;
            promoCode.UsageLimit = 1000;
            promoCode.ExpiresAt = DateTime.UtcNow.AddYears(1);
            promoCode.ApplicableCategoriesJson = $"[\"{ElectronicsCategoryId}\"]";
            promoCode.IsActive = true;
        }

        private async Task EnsureProductsAsync()
        {
            var mouse = await _context.Products
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(p => p.Id == MouseProductId || p.Slug == "seed-wireless-mouse");

            if (mouse == null)
            {
                mouse = new Product
                {
                    Id = MouseProductId,
                    Name = "Seed Wireless Mouse",
                    Slug = "seed-wireless-mouse",
                    Description = "Seeded product used for checkout and payment endpoint testing.",
                    Price = 450.00m,
                    DiscountedPrice = 399.00m,
                    Stock = 120,
                    SpecsJson = "{\"dpi\":\"1600\",\"connectivity\":\"2.4GHz\"}",
                    CategoryId = AccessoriesCategoryId,
                    SellerId = SellerProfileId,
                    IsDeleted = false
                };

                await _context.Products.AddAsync(mouse);
            }
            else
            {
                mouse.Name = "Seed Wireless Mouse";
                mouse.Slug = "seed-wireless-mouse";
                mouse.Description = "Seeded product used for checkout and payment endpoint testing.";
                mouse.Price = 450.00m;
                mouse.DiscountedPrice = 399.00m;
                mouse.Stock = 120;
                mouse.SpecsJson = "{\"dpi\":\"1600\",\"connectivity\":\"2.4GHz\"}";
                mouse.CategoryId = AccessoriesCategoryId;
                mouse.SellerId = SellerProfileId;
                mouse.IsDeleted = false;
                mouse.DeletedAt = null;
            }

            var keyboard = await _context.Products
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(p => p.Id == KeyboardProductId || p.Slug == "seed-mechanical-keyboard");

            if (keyboard == null)
            {
                keyboard = new Product
                {
                    Id = KeyboardProductId,
                    Name = "Seed Mechanical Keyboard",
                    Slug = "seed-mechanical-keyboard",
                    Description = "Seeded product for cart/order data and team API demos.",
                    Price = 1200.00m,
                    DiscountedPrice = 999.00m,
                    Stock = 80,
                    SpecsJson = "{\"switch\":\"red\",\"layout\":\"TKL\"}",
                    CategoryId = ElectronicsCategoryId,
                    SellerId = SellerProfileId,
                    IsDeleted = false
                };

                await _context.Products.AddAsync(keyboard);
            }
            else
            {
                keyboard.Name = "Seed Mechanical Keyboard";
                keyboard.Slug = "seed-mechanical-keyboard";
                keyboard.Description = "Seeded product for cart/order data and team API demos.";
                keyboard.Price = 1200.00m;
                keyboard.DiscountedPrice = 999.00m;
                keyboard.Stock = 80;
                keyboard.SpecsJson = "{\"switch\":\"red\",\"layout\":\"TKL\"}";
                keyboard.CategoryId = ElectronicsCategoryId;
                keyboard.SellerId = SellerProfileId;
                keyboard.IsDeleted = false;
                keyboard.DeletedAt = null;
            }
        }

        private async Task EnsureProductImagesAsync()
        {
            var mouseImage = await _context.ProductImages
                .FirstOrDefaultAsync(i => i.Id == MouseImageId || (i.ProductId == MouseProductId && i.DisplayOrder == 0));

            if (mouseImage == null)
            {
                await _context.ProductImages.AddAsync(new ProductImage
                {
                    Id = MouseImageId,
                    ProductId = MouseProductId,
                    ImageUrl = "https://picsum.photos/seed/seed-mouse/1200/900",
                    DisplayOrder = 0
                });
            }
            else
            {
                mouseImage.ProductId = MouseProductId;
                mouseImage.ImageUrl = "https://picsum.photos/seed/seed-mouse/1200/900";
                mouseImage.DisplayOrder = 0;
            }

            var keyboardImage = await _context.ProductImages
                .FirstOrDefaultAsync(i => i.Id == KeyboardImageId || (i.ProductId == KeyboardProductId && i.DisplayOrder == 0));

            if (keyboardImage == null)
            {
                await _context.ProductImages.AddAsync(new ProductImage
                {
                    Id = KeyboardImageId,
                    ProductId = KeyboardProductId,
                    ImageUrl = "https://picsum.photos/seed/seed-keyboard/1200/900",
                    DisplayOrder = 0
                });
            }
            else
            {
                keyboardImage.ProductId = KeyboardProductId;
                keyboardImage.ImageUrl = "https://picsum.photos/seed/seed-keyboard/1200/900";
                keyboardImage.DisplayOrder = 0;
            }
        }

        private async Task EnsureCustomerAddressAsync(string customerUserId)
        {
            var address = await _context.Addresses
                .FirstOrDefaultAsync(a => a.Id == CustomerAddressId || (a.UserId == customerUserId && a.Label == "Home"));

            if (address == null)
            {
                await _context.Addresses.AddAsync(new Address
                {
                    Id = CustomerAddressId,
                    Label = "Home",
                    Street = "10 Seed Street",
                    City = "Cairo",
                    PostalCode = "11511",
                    IsDefault = true,
                    UserId = customerUserId
                });
                return;
            }

            address.Label = "Home";
            address.Street = "10 Seed Street";
            address.City = "Cairo";
            address.PostalCode = "11511";
            address.IsDefault = true;
            address.UserId = customerUserId;
        }

        private async Task EnsureCustomerCartAsync(string customerUserId)
        {
            var cart = await _context.Carts
                .FirstOrDefaultAsync(c => c.Id == CustomerCartId || c.UserId == customerUserId);

            if (cart == null)
            {
                cart = new Cart
                {
                    Id = CustomerCartId,
                    UserId = customerUserId,
                    AppliedPromoCodeId = PromoCodeId,
                    UpdatedAt = DateTime.UtcNow
                };

                await _context.Carts.AddAsync(cart);
            }
            else
            {
                cart.UserId = customerUserId;
                cart.AppliedPromoCodeId = PromoCodeId;
                cart.UpdatedAt = DateTime.UtcNow;
            }

            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.Id == CustomerCartItemId || (ci.CartId == cart.Id && ci.ProductId == MouseProductId));

            if (cartItem == null)
            {
                await _context.CartItems.AddAsync(new CartItem
                {
                    Id = CustomerCartItemId,
                    CartId = cart.Id,
                    ProductId = MouseProductId,
                    Quantity = 2
                });
                return;
            }

            cartItem.CartId = cart.Id;
            cartItem.ProductId = MouseProductId;
            cartItem.Quantity = 2;
        }

        private async Task EnsureOrdersAsync(string customerUserId)
        {
            var stripeOrder = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == StripePendingOrderId || o.OrderNumber == "ORD-SEED-STRIPE-1001");

            if (stripeOrder == null)
            {
                stripeOrder = new Order
                {
                    Id = StripePendingOrderId,
                    OrderNumber = "ORD-SEED-STRIPE-1001",
                    Status = OrderStatus.Pending,
                    PaymentMethod = PaymentMethod.Stripe,
                    PaymentStatus = PaymentStatus.Pending,
                    Total = 399.00m,
                    Notes = "Seed stripe pending order",
                    ShippingStreet = "10 Seed Street",
                    ShippingCity = "Cairo",
                    ShippingPostalCode = "11511",
                    PromoCodeId = PromoCodeId,
                    UserId = customerUserId
                };
                await _context.Orders.AddAsync(stripeOrder);
            }
            else
            {
                stripeOrder.OrderNumber = "ORD-SEED-STRIPE-1001";
                stripeOrder.Status = OrderStatus.Pending;
                stripeOrder.PaymentMethod = PaymentMethod.Stripe;
                stripeOrder.PaymentStatus = PaymentStatus.Pending;
                stripeOrder.Total = 399.00m;
                stripeOrder.Notes = "Seed stripe pending order";
                stripeOrder.ShippingStreet = "10 Seed Street";
                stripeOrder.ShippingCity = "Cairo";
                stripeOrder.ShippingPostalCode = "11511";
                stripeOrder.PromoCodeId = PromoCodeId;
                stripeOrder.UserId = customerUserId;
            }

            var stripeOrderItem = await _context.OrderItems
                .FirstOrDefaultAsync(i => i.Id == StripePendingOrderItemId || (i.OrderId == stripeOrder.Id && i.ProductId == MouseProductId));

            if (stripeOrderItem == null)
            {
                await _context.OrderItems.AddAsync(new OrderItem
                {
                    Id = StripePendingOrderItemId,
                    OrderId = stripeOrder.Id,
                    ProductId = MouseProductId,
                    Quantity = 1,
                    UnitPrice = 399.00m,
                    ProductName = "Seed Wireless Mouse"
                });
            }
            else
            {
                stripeOrderItem.OrderId = stripeOrder.Id;
                stripeOrderItem.ProductId = MouseProductId;
                stripeOrderItem.Quantity = 1;
                stripeOrderItem.UnitPrice = 399.00m;
                stripeOrderItem.ProductName = "Seed Wireless Mouse";
            }

            var stripeHistory = await _context.OrderStatusHistories
                .FirstOrDefaultAsync(h => h.Id == StripePendingStatusHistoryId || (h.OrderId == stripeOrder.Id && h.Status == OrderStatus.Pending));

            if (stripeHistory == null)
            {
                await _context.OrderStatusHistories.AddAsync(new OrderStatusHistory
                {
                    Id = StripePendingStatusHistoryId,
                    OrderId = stripeOrder.Id,
                    Status = OrderStatus.Pending,
                    Timestamp = DateTime.UtcNow
                });
            }

            var paidOrder = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == PaidOrderId || o.OrderNumber == "ORD-SEED-PAID-1002");

            if (paidOrder == null)
            {
                paidOrder = new Order
                {
                    Id = PaidOrderId,
                    OrderNumber = "ORD-SEED-PAID-1002",
                    Status = OrderStatus.Completed,
                    PaymentMethod = PaymentMethod.Stripe,
                    PaymentStatus = PaymentStatus.Paid,
                    Total = 999.00m,
                    Notes = "Seed paid order",
                    ShippingStreet = "10 Seed Street",
                    ShippingCity = "Cairo",
                    ShippingPostalCode = "11511",
                    UserId = customerUserId,
                    EstimatedDelivery = DateTime.UtcNow.AddDays(3)
                };
                await _context.Orders.AddAsync(paidOrder);
            }
            else
            {
                paidOrder.OrderNumber = "ORD-SEED-PAID-1002";
                paidOrder.Status = OrderStatus.Completed;
                paidOrder.PaymentMethod = PaymentMethod.Stripe;
                paidOrder.PaymentStatus = PaymentStatus.Paid;
                paidOrder.Total = 999.00m;
                paidOrder.Notes = "Seed paid order";
                paidOrder.ShippingStreet = "10 Seed Street";
                paidOrder.ShippingCity = "Cairo";
                paidOrder.ShippingPostalCode = "11511";
                paidOrder.UserId = customerUserId;
                paidOrder.EstimatedDelivery = DateTime.UtcNow.AddDays(3);
            }

            var paidOrderItem = await _context.OrderItems
                .FirstOrDefaultAsync(i => i.Id == PaidOrderItemId || (i.OrderId == paidOrder.Id && i.ProductId == KeyboardProductId));

            if (paidOrderItem == null)
            {
                await _context.OrderItems.AddAsync(new OrderItem
                {
                    Id = PaidOrderItemId,
                    OrderId = paidOrder.Id,
                    ProductId = KeyboardProductId,
                    Quantity = 1,
                    UnitPrice = 999.00m,
                    ProductName = "Seed Mechanical Keyboard"
                });
            }
            else
            {
                paidOrderItem.OrderId = paidOrder.Id;
                paidOrderItem.ProductId = KeyboardProductId;
                paidOrderItem.Quantity = 1;
                paidOrderItem.UnitPrice = 999.00m;
                paidOrderItem.ProductName = "Seed Mechanical Keyboard";
            }

            var paidHistory = await _context.OrderStatusHistories
                .FirstOrDefaultAsync(h => h.Id == PaidStatusHistoryId || (h.OrderId == paidOrder.Id && h.Status == OrderStatus.Completed));

            if (paidHistory == null)
            {
                await _context.OrderStatusHistories.AddAsync(new OrderStatusHistory
                {
                    Id = PaidStatusHistoryId,
                    OrderId = paidOrder.Id,
                    Status = OrderStatus.Completed,
                    Timestamp = DateTime.UtcNow
                });
            }
        }
    }
}
