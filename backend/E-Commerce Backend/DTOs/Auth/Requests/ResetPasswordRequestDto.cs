using System.ComponentModel.DataAnnotations;

namespace E_Commerce_Backend.DTOs.Auth.Requests
{
    // POST /auth/reset-password
    public class ResetPasswordRequestDto
    {
        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Token is required")]
        public string Token { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "New password is required")] 
        public string NewPassword { get; set; } = string.Empty;
    }
}
