namespace E_Commerce_Backend.Models
{
    public class Cart
    {
        public Guid Id { get; set; }

        // Source: API Contract → guest carts use session ID; nullable for guest carts
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }

        // Source: API Contract Headers → X-Guest-Session: {uuid}
        public string? GuestSessionId { get; set; }

        // Source: API Contract GET /cart → appliedPromoCode (nullable)
        public Guid? AppliedPromoCodeId { get; set; }
        public PromoCode? AppliedPromoCode { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // ── Navigation ──────────────────────────────────────────────────────
        public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
    }


}

