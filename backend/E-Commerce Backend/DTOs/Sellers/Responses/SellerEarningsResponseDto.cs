namespace E_Commerce_Backend.DTOs.Sellers.Responses
{
    // GET /sellers/me/earnings (Bonus)
    public class SellerEarningsResponseDto
    {
        public decimal TotalEarnings { get; set; }
        public decimal PendingPayout { get; set; }
        public decimal PaidOut { get; set; }
        // Transactions: NOT DEFINED IN SRS/API CONTRACT (structure not specified)
        // Placeholder list — structure to be defined when transaction entity is designed
        public List<object> Transactions { get; set; } = new();
    }
}
