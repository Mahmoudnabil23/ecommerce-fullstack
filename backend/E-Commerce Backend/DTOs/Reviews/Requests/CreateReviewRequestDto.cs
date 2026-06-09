namespace E_Commerce_Backend.DTOs.Reviews.Requests
{

    // POST /products/{productId}/reviews
    public class CreateReviewRequestDto
    {
        public int Rating { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        // images: NOT DEFINED as file upload or URL list in API Contract for this endpoint
        // Storing as optional list of URL strings per response schema
        public List<string>? Images { get; set; }
    }
}
