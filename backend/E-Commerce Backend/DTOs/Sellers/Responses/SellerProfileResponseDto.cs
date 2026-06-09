using E_Commerce_Backend.Enums;

namespace E_Commerce_Backend.DTOs.Sellers.Responses
{

    // GET /sellers/me  |  GET /sellers/{id}
    public class SellerProfileResponseDto
    {
        public Guid Id { get; set; }
        public string StoreName { get; set; } = string.Empty;
        public string? StoreDescription { get; set; }
        public SellerStatus Status { get; set; }
        public decimal AverageRating { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
