namespace E_Commerce_Backend.Models
{
    public class Banner
    {
        public Guid Id { get; set; }

        // Source: API Contract → title
        public string Title { get; set; } = string.Empty;

        // Source: API Contract → imageFile → stored as URL after upload
        public string ImageUrl { get; set; } = string.Empty;

        // Source: API Contract → linkUrl
        public string? LinkUrl { get; set; }

        // Source: API Contract → position
        public int Position { get; set; }

        // Source: API Contract → isActive
        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }


}

