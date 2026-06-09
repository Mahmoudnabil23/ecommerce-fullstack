using E_Commerce_Backend.Enums;

namespace E_Commerce_Backend.DTOs.Orders.Requests
{
    // PUT /orders/{id}/status
    public class UpdateOrderStatusRequestDto
    {
        public OrderStatus Status { get; set; }
        public string? TrackingNumber { get; set; }
    }
}
