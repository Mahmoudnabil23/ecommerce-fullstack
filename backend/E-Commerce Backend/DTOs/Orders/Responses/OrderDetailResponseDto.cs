using E_Commerce_Backend.Enums;

namespace E_Commerce_Backend.DTOs.Orders.Responses
{
    // GET /orders/{id}
    public class OrderDetailResponseDto
    {
        public Guid OrderId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public OrderStatus Status { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
        public OrderAddressDto Address { get; set; } = new();
        public PaymentMethod PaymentMethod { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public string? TrackingNumber { get; set; }
        public List<OrderStatusHistoryDto> StatusHistory { get; set; } = new();
        public decimal Total { get; set; }
    }
}
