namespace E_Commerce_Backend.DTOs.Admin.Requests
{
    // POST /admin/banners  (multipart/form-data — imageFile handled in controller)
    public class CreateBannerRequestDto
    {
        public string Title { get; set; } = string.Empty;
        public string? LinkUrl { get; set; }
        public int Position { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
