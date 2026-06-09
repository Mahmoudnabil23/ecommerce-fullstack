namespace E_Commerce_Backend.DTOs.Reviews.Responses
{
    public class ReviewItemDto
    {
        public Guid Id { get; set; }
        public ReviewUserDto User { get; set; } = new();
        public int Rating { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public List<string> Images { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public bool IsVerifiedPurchase { get; set; }
    }
}
