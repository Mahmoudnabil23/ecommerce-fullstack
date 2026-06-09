using E_Commerce_Backend.Enums;

namespace E_Commerce_Backend.Models
{
    public class SellerProfile
    {
        public Guid Id { get; set; }

        // Source: API Contract POST /sellers/register → storeName
        public string StoreName { get; set; } = string.Empty;

        // Source: API Contract POST /sellers/register → storeDescription
        public string? StoreDescription { get; set; }

        // Source: API Contract POST /sellers/register → taxId
        public string? TaxId { get; set; }

        // Source: API Contract PUT /admin/sellers/{id}/verify → verified bool
        public SellerStatus Status { get; set; } = SellerStatus.Pending;

        // Source: API Contract GET /sellers/me → rating (public store page)
        // Computed/cached value — NOT DEFINED as formula in SRS/API Contract
        public decimal AverageRating { get; set; } = 0;

        // Source: API Contract POST /sellers/register → bankAccount object
        public string? BankName { get; set; }
        public string? BankAccountNumber { get; set; }
        public string? BankAccountHolderName { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // ── Foreign Key (1-to-1 with ApplicationUser) ───────────────────────
        public string UserId { get; set; }
        public ApplicationUser User { get; set; } = null!;

        // ── Navigation ──────────────────────────────────────────────────────
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }


}

