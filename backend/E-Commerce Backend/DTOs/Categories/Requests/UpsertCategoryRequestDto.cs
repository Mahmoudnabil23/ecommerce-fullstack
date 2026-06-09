namespace E_Commerce_Backend.DTOs.Categories.Requests
{
    // POST /categories  |  PUT /categories/{id}
    public class UpsertCategoryRequestDto
    {
        public string Name { get; set; } = string.Empty;
        public Guid? ParentId { get; set; }
        public string? ImageUrl { get; set; }
    }
}
