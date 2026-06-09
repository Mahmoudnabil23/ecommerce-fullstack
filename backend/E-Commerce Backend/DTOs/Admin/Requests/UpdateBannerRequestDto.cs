namespace E_Commerce_Backend.DTOs.Admin.Requests
{
    // PUT /admin/banners/{id}
    public class UpdateBannerRequestDto
    {
        public string? Title { get; set; }
        public string? LinkUrl { get; set; }
        public int? Position { get; set; }
        public bool? IsActive { get; set; }
    }
}
