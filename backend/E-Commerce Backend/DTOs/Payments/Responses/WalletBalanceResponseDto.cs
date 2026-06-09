namespace E_Commerce_Backend.DTOs.Payments.Responses
{
    // GET /payments/wallet/balance (Bonus)
    public class WalletBalanceResponseDto
    {
        public decimal Balance { get; set; }
        public string Currency { get; set; } = "EGP";
    }
}
