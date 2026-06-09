using E_Commerce_Backend.Enums;

namespace E_Commerce_Backend.DTOs.Orders.Responses
{
    public class OrderStatusHistoryDto
    {
        public OrderStatus Status { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
