namespace E_Commerce_Backend.DTOs.Cart.Requests
{
    // POST /cart/promo
    public class ApplyPromoCodeRequestDto
    {
        public string Code { get; set; } = string.Empty;
    }
}
