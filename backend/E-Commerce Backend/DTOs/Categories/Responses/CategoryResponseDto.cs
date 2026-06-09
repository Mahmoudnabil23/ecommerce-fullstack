namespace E_Commerce_Backend.DTOs.Categories.Responses
{
    // GET /categories → tree node
    public class CategoryResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public List<CategoryResponseDto> Children { get; set; } = new();
    }
}
