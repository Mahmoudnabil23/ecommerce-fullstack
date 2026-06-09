namespace E_Commerce_Backend.DTOs.Products.Requests
{
    // PUT /products/{id}
    public class UpdateProductRequestDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public Guid? CategoryId { get; set; }
        public int? Stock { get; set; }
        public string? SpecsJson { get; set; }
    }
}
