using E_Commerce_Backend.Enums;

namespace E_Commerce_Backend.DTOs.Payments.Requests
{

    // POST /payments/initiate
    public class InitiatePaymentRequestDto
    {
        public Guid OrderId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public string Currency { get; set; } = "EGP";
    }
}
