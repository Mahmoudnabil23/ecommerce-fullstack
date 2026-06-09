using E_Commerce_Backend.Enums;

namespace E_Commerce_Backend.DTOs.Admin.Requests
{
    // POST /admin/promo-codes  |  PUT /admin/promo-codes/{id}
    public class UpsertPromoCodeRequestDto
    {
        public string Code { get; set; } = string.Empty;
        public PromoCodeType Type { get; set; }
        public decimal Value { get; set; }
        public decimal MinOrderAmount { get; set; }
        public int UsageLimit { get; set; }
        public DateTime ExpiresAt { get; set; }
        public List<Guid>? ApplicableCategories { get; set; }
    }
}
