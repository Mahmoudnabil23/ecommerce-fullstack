namespace E_Commerce_Backend.DTOs.Cart.Responses
{
    public class CartProductRefDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public List<string> ImageUrls { get; set; } = new List<string>();
    }
}
