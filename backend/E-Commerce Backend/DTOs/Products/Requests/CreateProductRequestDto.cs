using System.ComponentModel.DataAnnotations;

namespace E_Commerce_Backend.DTOs.Products.Requests
{

    // POST /products  (multipart/form-data — image files handled separately)
    public class CreateProductRequestDto
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Description { get; set; } = string.Empty;
        [Required]
        public decimal Price { get; set; }
        public Guid? CategoryId { get; set; }
        public int Stock { get; set; }
        public string? SpecsJson { get; set; }
        // Images are IFormFile[] — handled in controller, not in this DTO
    }
}
