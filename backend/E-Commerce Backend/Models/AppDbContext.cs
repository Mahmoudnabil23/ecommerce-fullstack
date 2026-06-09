using E_Commerce_Backend.Enums;
using E_Commerce_Backend.Models.Configurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce_Backend.Models
{
        public class AppDbContext : IdentityDbContext<ApplicationUser>
        {
            public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
            
            // ── DbSets ────────────────────────────────────────────────────────
            public DbSet<RefreshToken> RefreshTokens { get; set; }
            public DbSet<Address> Addresses { get; set; }
            public DbSet<Category> Categories { get; set; }
            public DbSet<Product> Products { get; set; }
            public DbSet<ProductImage> ProductImages { get; set; }
            public DbSet<SellerProfile> SellerProfiles { get; set; }
            public DbSet<Cart> Carts { get; set; }
            public DbSet<CartItem> CartItems { get; set; }
            public DbSet<Order> Orders { get; set; }
            public DbSet<OrderItem> OrderItems { get; set; }
            public DbSet<OrderStatusHistory> OrderStatusHistories { get; set; }
            public DbSet<Review> Reviews { get; set; }
            public DbSet<WishlistItem> WishlistItems { get; set; }
            public DbSet<PromoCode> PromoCodes { get; set; }
            public DbSet<Banner> Banners { get; set; }
            public DbSet<Notification> Notifications { get; set; }

            protected override void OnModelCreating(ModelBuilder builder)
            {
                base.OnModelCreating(builder);

                // ── Apply global query filters for soft delete ──────────────────────────
                builder.Entity<Product>().HasQueryFilter(p => !p.IsDeleted);
                builder.Entity<Category>().HasQueryFilter(c => !c.IsDeleted);
                builder.Entity<ApplicationUser>().HasQueryFilter(u => !u.IsDeleted);


                // ── Apply all entity configurations ──────────────────────────
                builder.ApplyConfiguration(new ApplicationUserConfiguration());
                builder.ApplyConfiguration(new AddressConfiguration());
                builder.ApplyConfiguration(new CategoryConfiguration());
                builder.ApplyConfiguration(new ProductConfiguration());
                builder.ApplyConfiguration(new ProductImageConfiguration());
                builder.ApplyConfiguration(new SellerProfileConfiguration());
                builder.ApplyConfiguration(new CartConfiguration());
                builder.ApplyConfiguration(new CartItemConfiguration());
                builder.ApplyConfiguration(new OrderConfiguration());
                builder.ApplyConfiguration(new OrderItemConfiguration());
                builder.ApplyConfiguration(new OrderStatusHistoryConfiguration());
                builder.ApplyConfiguration(new ReviewConfiguration());
                builder.ApplyConfiguration(new WishlistItemConfiguration());
                builder.ApplyConfiguration(new PromoCodeConfiguration());
                builder.ApplyConfiguration(new BannerConfiguration());
                builder.ApplyConfiguration(new NotificationConfiguration());
            }
        }


     

}


