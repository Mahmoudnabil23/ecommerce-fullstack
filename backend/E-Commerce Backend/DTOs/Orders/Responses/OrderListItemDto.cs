using E_Commerce_Backend.Enums;

namespace E_Commerce_Backend.DTOs.Orders.Responses
{
    // GET /orders (list item — shorter)
    public class OrderListItemDto
    {
        public Guid OrderId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public OrderStatus Status { get; set; }
        public decimal Total { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
