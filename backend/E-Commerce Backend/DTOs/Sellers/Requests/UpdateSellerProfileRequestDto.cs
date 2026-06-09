namespace E_Commerce_Backend.DTOs.Sellers.Requests
{
    // PUT /sellers/me
    public class UpdateSellerProfileRequestDto
    {
        public string? StoreName { get; set; }
        public string? StoreDescription { get; set; }
    }
}
