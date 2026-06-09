using E_Commerce_Backend.Enums;

namespace E_Commerce_Backend.DTOs.PromoCodes.Responses
{
    public class PromoCodeResponseDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public PromoCodeType Type { get; set; }
        public decimal Value { get; set; }
        public decimal MinOrderAmount { get; set; }
        public int UsageLimit { get; set; }
        public int UsageCount { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsActive { get; set; }
    }
}
