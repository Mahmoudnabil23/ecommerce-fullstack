namespace E_Commerce_Backend.DTOs.Auth.Requests
{
    // POST /auth/refresh-token
    public class RefreshTokenRequestDto
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}
