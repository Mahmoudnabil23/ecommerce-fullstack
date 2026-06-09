namespace E_Commerce_Backend.DTOs.Orders.Responses
{
    public class OrderItemDto
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Qty { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
