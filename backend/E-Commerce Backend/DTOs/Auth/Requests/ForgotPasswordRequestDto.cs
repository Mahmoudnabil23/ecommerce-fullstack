namespace E_Commerce_Backend.DTOs.Auth.Requests
{
    // POST /auth/forgot-password
    public class ForgotPasswordRequestDto
    {
        public string Email { get; set; } = string.Empty;
    }
}
