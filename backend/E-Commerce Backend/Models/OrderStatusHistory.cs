using E_Commerce_Backend.Enums;

namespace E_Commerce_Backend.Models
{
    public class OrderStatusHistory
    {
        public Guid Id { get; set; }

        // Source: API Contract → status
        public OrderStatus Status { get; set; }

        // Source: API Contract → timestamp
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        // ── Foreign Key ─────────────────────────────────────────────────────
        public Guid OrderId { get; set; }
        public Order Order { get; set; } = null!;
    }


}

