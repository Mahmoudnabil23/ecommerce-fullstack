using E_Commerce_Backend.Enums;

namespace E_Commerce_Backend.Models
{
    public class Order
    {
        public Guid Id { get; set; }

        // Source: API Contract → orderNumber (e.g., "ORD-20240115-00042")
        public string OrderNumber { get; set; } = string.Empty;

        // Source: API Contract → status
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        // Source: API Contract GET /orders/{id} → paymentMethod
        public PaymentMethod PaymentMethod { get; set; }

        // Source: API Contract GET /orders/{id} → paymentStatus
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

        // Source: API Contract GET /orders/{id} → trackingNumber
        public string? TrackingNumber { get; set; }

        // Source: API Contract POST /orders → notes
        public string? Notes { get; set; }

        // Source: API Contract GET /orders/{id} → total
        public decimal Total { get; set; }

        // Source: API Contract POST /orders → promoCode applied
        public Guid? PromoCodeId { get; set; }
        public PromoCode? PromoCode { get; set; }

        // Source: API Contract → estimatedDelivery
        // NOT DEFINED IN SRS/API CONTRACT as a calculation rule; stored as provided
        public DateTime? EstimatedDelivery { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // ── Shipping Address Snapshot ────────────────────────────────────────
        // Snapshot (not FK) — address must not change after order placed
        // Source: API Contract GET /orders/{id} → address object
        public string ShippingStreet { get; set; } = string.Empty;
        public string ShippingCity { get; set; } = string.Empty;
        public string? ShippingPostalCode { get; set; }

        // ── Foreign Keys ────────────────────────────────────────────────────
        // Nullable: guest checkout allowed per SRS §3
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }

        // Source: API Contract POST /orders → guest requires email
        public string? GuestEmail { get; set; }

        // ── Navigation ──────────────────────────────────────────────────────
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
        public ICollection<OrderStatusHistory> StatusHistory { get; set; } = new List<OrderStatusHistory>();
    }


}

