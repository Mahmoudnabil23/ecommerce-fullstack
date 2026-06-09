namespace E_Commerce_Backend.DTOs.Auth.Requests
{
    // POST /auth/login
    public class LoginRequestDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
