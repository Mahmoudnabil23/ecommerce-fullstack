namespace E_Commerce_Backend.DTOs.Payments.Responses
{

    // POST /payments/initiate
    public class PaymentInitiateResponseDto
    {
        public string ClientSecret { get; set; } = string.Empty;
        public string PaymentIntentId { get; set; } = string.Empty;
    }
}
