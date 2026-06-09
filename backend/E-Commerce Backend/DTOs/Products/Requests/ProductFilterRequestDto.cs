namespace E_Commerce_Backend.DTOs.Products.Requests
{
    // GET /products — query parameters
    public class ProductFilterRequestDto
    {
        public string? Search { get; set; }
        public Guid? CategoryId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public Guid? SellerId { get; set; }
        public bool? InStock { get; set; }
        public string? SortBy { get; set; }  // price_asc | price_desc | newest | rating
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 20;
    }
}
