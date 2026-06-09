namespace E_Commerce_Backend.DTOs.Cart.Responses
{

    // GET /cart
    public class CartResponseDto
    {
        public Guid CartId { get; set; }
        public List<CartItemDto> Items { get; set; } = new();
        public CartSummaryDto Summary { get; set; } = new();
        public string? AppliedPromoCode { get; set; }
    }
}
