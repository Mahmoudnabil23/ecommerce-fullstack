using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace E_Commerce_Backend.Models
{
    public class Product
    {
        public Guid Id { get; set; }

        // Source: API Contract GET /products → name
       
        public string Name { get; set; } = string.Empty;

        // Source: API Contract GET /products → slug
        public string? Slug { get; set; } = string.Empty;

        // Source: SRS §2 → descriptions
        public string Description { get; set; } = string.Empty;

        // Source: API Contract GET /products → price
        public decimal Price { get; set; }

        // Source: API Contract GET /products → discountedPrice
        public decimal? DiscountedPrice { get; set; }

        // Source: SRS §2 → stock availability
        public int Stock { get; set; }

        // Source: API Contract GET /products → specs (JSON string in request)
        // Stored as serialized JSON; NOT DEFINED as structured sub-entity in SRS/API Contract
        public string? SpecsJson { get; set; }

        // Source: SRS §2 → soft delete implied by admin soft-delete policy
        
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // ── Foreign Keys ────────────────────────────────────────────────────
        public Guid? CategoryId { get; set; }
        public Category? Category { get; set; } = null!;

        public Guid? SellerId { get; set; }
        public SellerProfile? Seller { get; set; } = null!;

        // ── Navigation ──────────────────────────────────────────────────────
        public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public ICollection<WishlistItem> WishlistItems { get; set; } = new List<WishlistItem>();
    }

}
