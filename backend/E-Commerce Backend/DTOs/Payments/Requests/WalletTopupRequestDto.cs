using E_Commerce_Backend.Enums;

namespace E_Commerce_Backend.DTOs.Payments.Requests
{
    // POST /payments/wallet/topup (Bonus)
    public class WalletTopupRequestDto
    {
        public decimal Amount { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public string? PaymentToken { get; set; }
    }
}
