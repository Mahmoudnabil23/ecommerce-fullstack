namespace E_Commerce_Backend.DTOs.Auth.Responses
{

    // POST /auth/login | POST /auth/register (on login) | POST /auth/refresh-token
    public class AuthResponseDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public int ExpiresIn { get; set; }  // seconds
        public UserSummaryDto User { get; set; } = new();
    }
}
