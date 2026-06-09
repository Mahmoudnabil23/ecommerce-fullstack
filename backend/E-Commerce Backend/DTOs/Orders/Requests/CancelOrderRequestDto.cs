namespace E_Commerce_Backend.DTOs.Orders.Requests
{
    // POST /orders/{id}/cancel
    public class CancelOrderRequestDto
    {
        public string? Reason { get; set; }
    }
}
