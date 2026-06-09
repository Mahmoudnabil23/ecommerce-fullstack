using E_Commerce_Backend.Enums;

namespace E_Commerce_Backend.DTOs.Orders.Responses
{

    // POST /orders (201)
    public class OrderCreatedResponseDto
    {
        public Guid OrderId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public OrderStatus Status { get; set; }
        public decimal Total { get; set; }
        public string PaymentStatus { get; set; } = string.Empty;
        public DateTime? EstimatedDelivery { get; set; }
    }
}
