using E_Commerce_Backend.Enums;

namespace E_Commerce_Backend.DTOs.Sellers.Responses
{
    // GET /admin/sellers list item
    public class AdminSellerListItemDto
    {
        public Guid Id { get; set; }
        public string StoreName { get; set; } = string.Empty;
        public string OwnerEmail { get; set; } = string.Empty;
        public SellerStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
