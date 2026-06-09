using E_Commerce_Backend.DTOs.Products.Requests;

namespace E_Commerce_Backend.DTOs.Admin.Requests
{
    public class AdminProductFilterRequestDto : ProductFilterRequestDto
    {
        public bool? IsDeleted { get; set; }
    }
}
