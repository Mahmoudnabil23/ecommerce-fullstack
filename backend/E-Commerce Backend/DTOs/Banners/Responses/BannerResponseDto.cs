namespace E_Commerce_Backend.DTOs.Banners.Responses
{
    public class BannerResponseDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string? LinkUrl { get; set; }
        public int Position { get; set; }
        public bool IsActive { get; set; }
    }
}
