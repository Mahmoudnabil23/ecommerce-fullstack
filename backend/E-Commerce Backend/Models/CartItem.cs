namespace E_Commerce_Backend.Models
{
    public class CartItem
    {
        public Guid Id { get; set; }

        // Source: API Contract GET /cart → quantity
        public int Quantity { get; set; }

        // ── Foreign Keys ────────────────────────────────────────────────────
        public Guid CartId { get; set; }
        public Cart Cart { get; set; } = null!;

        public Guid ProductId { get; set; }
        public Product Product { get; set; } = null!;
    }


}

