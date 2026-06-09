namespace E_Commerce_Backend.Models
{
    public class WishlistItem
    {
        public Guid Id { get; set; }

        public DateTime AddedAt { get; set; } = DateTime.UtcNow;

        // ── Foreign Keys ────────────────────────────────────────────────────
        public string UserId { get; set; }
        public ApplicationUser User { get; set; } = null!;

        public Guid ProductId { get; set; }
        public Product Product { get; set; } = null!;
    }


}

