namespace E_Commerce_Backend.DTOs.Auth.Requests
{
    // POST /auth/change-password
    public class ChangePasswordRequestDto
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
