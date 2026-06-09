namespace E_Commerce_Backend.DTOs.Cart.Requests
{
    // POST /cart/merge
    public class MergeCartRequestDto
    {
        public string GuestSessionId { get; set; } = string.Empty;
    }
}
