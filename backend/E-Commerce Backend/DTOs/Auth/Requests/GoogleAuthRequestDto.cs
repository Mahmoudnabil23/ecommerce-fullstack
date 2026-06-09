namespace E_Commerce_Backend.DTOs.Auth.Requests
{
    // POST /auth/google (Bonus)
    public class GoogleAuthRequestDto
    {
        public string IdToken { get; set; } = string.Empty;
    }
}
