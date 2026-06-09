namespace E_Commerce_Backend.DTOs.Cart.Responses
{
    public class CartSummaryDto
    {
        public decimal Subtotal { get; set; }
        public decimal Discount { get; set; }
        public decimal Shipping { get; set; }
        public decimal Total { get; set; }
    }
}
