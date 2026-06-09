using E_Commerce_Backend.Models;

namespace E_Commerce_Backend.DTOs.Products.Responses
{

    // GET /products → items array item
    public class ProductListItemDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Slug { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal? DiscountedPrice { get; set; }
        public List<string> Images { get; set; } = new();
        public CategoryRefDto Category { get; set; } = new();
        public SellerRefDto Seller { get; set; } = new();
        public int Stock { get; set; }
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
    }
}
