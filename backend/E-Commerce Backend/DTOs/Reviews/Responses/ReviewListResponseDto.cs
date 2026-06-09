namespace E_Commerce_Backend.DTOs.Reviews.Responses
{

    // GET /products/{productId}/reviews
    public class ReviewListResponseDto
    {
        public decimal AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public ReviewDistributionDto Distribution { get; set; } = new();
        public List<ReviewItemDto> Reviews { get; set; } = new();
    }
}
