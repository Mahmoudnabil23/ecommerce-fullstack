namespace E_Commerce_Backend.DTOs.Orders.Responses
{
    public class OrderAddressDto
    {
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string? PostalCode { get; set; }
    }
}
