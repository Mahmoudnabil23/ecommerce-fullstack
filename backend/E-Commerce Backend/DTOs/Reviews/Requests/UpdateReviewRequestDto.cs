namespace E_Commerce_Backend.DTOs.Reviews.Requests
{
    // PUT /products/{productId}/reviews/{reviewId}
    public class UpdateReviewRequestDto
    {
        public int? Rating { get; set; }
        public string? Title { get; set; }
        public string? Body { get; set; }
    }
}
