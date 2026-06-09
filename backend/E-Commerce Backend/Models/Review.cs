namespace E_Commerce_Backend.Models
{
    public class Review
    {
        public Guid Id { get; set; }

        // Source: API Contract GET /products/{id}/reviews → rating (1–5)
        public int Rating { get; set; }

        // Source: API Contract → title
        public string Title { get; set; } = string.Empty;

        // Source: API Contract → body
        public string Body { get; set; } = string.Empty;

        // Source: API Contract → images (array of URLs)
        // Stored as JSON array string; no separate ReviewImage entity defined in SRS/API Contract
        public string? ImagesJson { get; set; }

        // Source: API Contract → isVerifiedPurchase
        public bool IsVerifiedPurchase { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // ── Foreign Keys ────────────────────────────────────────────────────
        public string UserId { get; set; }
        public ApplicationUser User { get; set; } = null!;

        public Guid ProductId { get; set; }
        public Product Product { get; set; } = null!;
    }


}

