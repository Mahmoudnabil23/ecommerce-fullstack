namespace E_Commerce_Backend.Models
{
    public class OrderItem
    {
        public Guid Id { get; set; }

        // Source: API Contract → qty
        public int Quantity { get; set; }

        // Source: API Contract → unitPrice (snapshot at time of order)
        public decimal UnitPrice { get; set; }

        // Source: API Contract → name (snapshot — product name at order time)
        public string ProductName { get; set; } = string.Empty;

        // ── Foreign Keys ────────────────────────────────────────────────────
        public Guid OrderId { get; set; }
        public Order Order { get; set; } = null!;

        public Guid ProductId { get; set; }
        public Product Product { get; set; } = null!;
    }


}

