namespace E_Commerce_Backend.Models
{
    public class Notification
    {
        public Guid Id { get; set; }

        // Source: API Contract GET /notifications → type (order updates, promotions)
        // NOT DEFINED as enum in SRS/API Contract — stored as string
        public string Type { get; set; } = string.Empty;

        // Source: API Contract → message/body content
        // NOT DEFINED as explicit field name in SRS/API Contract
        public string Message { get; set; } = string.Empty;

        // Source: API Contract GET /notifications?read=false → read flag
        public bool IsRead { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // ── Foreign Key ─────────────────────────────────────────────────────
        public string UserId { get; set; }
        public ApplicationUser User { get; set; } = null!;
    }


}

