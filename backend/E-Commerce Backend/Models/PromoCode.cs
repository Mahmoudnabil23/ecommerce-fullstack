using E_Commerce_Backend.Enums;

namespace E_Commerce_Backend.Models
{
    public class PromoCode
    {
        public Guid Id { get; set; }

        // Source: API Contract → code
        public string Code { get; set; } = string.Empty;

        // Source: API Contract → type: "percentage" | implicit "fixed"
        public PromoCodeType Type { get; set; }

        // Source: API Contract → value
        public decimal Value { get; set; }

        // Source: API Contract → minOrderAmount
        public decimal MinOrderAmount { get; set; }

        // Source: API Contract → usageLimit
        public int UsageLimit { get; set; }

        // Tracks current usage — required by usageLimit enforcement
        public int UsageCount { get; set; } = 0;

        // Source: API Contract → expiresAt
        public DateTime ExpiresAt { get; set; }

        // Source: API Contract → applicableCategories (array of category IDs)
        // Stored as JSON; no join table defined in SRS/API Contract
        public string? ApplicableCategoriesJson { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }


}

