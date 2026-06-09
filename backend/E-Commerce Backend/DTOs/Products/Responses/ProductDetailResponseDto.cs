namespace E_Commerce_Backend.DTOs.Products.Responses
{
    // GET /products/{id} — full detail
    public class ProductDetailResponseDto : ProductListItemDto
    {
        public string Description { get; set; } = string.Empty;
        public string? SpecsJson { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
