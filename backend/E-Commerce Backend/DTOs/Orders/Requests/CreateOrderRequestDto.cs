using E_Commerce_Backend.Enums;

namespace E_Commerce_Backend.DTOs.Orders.Requests
{
    // POST /orders
    public class CreateOrderRequestDto
    {
        public Guid? AddressId { get; set; }   // Nullable: guest may provide inline address
        
        // Inline address fields (if AddressId is null)
        public string? FullName { get; set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? PostalCode { get; set; }
        public string? Phone { get; set; }

        public PaymentMethod PaymentMethod { get; set; }
        public string? PaymentToken { get; set; }
        public string? PromoCode { get; set; }
        public string? Notes { get; set; }
    }
}
