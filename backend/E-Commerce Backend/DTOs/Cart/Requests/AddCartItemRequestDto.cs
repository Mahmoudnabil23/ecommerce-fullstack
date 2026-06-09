namespace E_Commerce_Backend.DTOs.Cart.Requests
{

    // POST /cart/items
    public class AddCartItemRequestDto
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
