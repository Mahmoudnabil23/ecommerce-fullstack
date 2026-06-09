namespace E_Commerce_Backend.DTOs.Auth.Requests
{
    // POST /auth/logout
    public class LogoutRequestDto
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}
