namespace E_Commerce_Backend.DTOs.Cart.Responses
{
    public class CartItemDto
    {
        public Guid CartItemId { get; set; }
        public CartProductRefDto Product { get; set; } = new();
        public int Quantity { get; set; }
        public decimal Subtotal { get; set; }
    }
}
