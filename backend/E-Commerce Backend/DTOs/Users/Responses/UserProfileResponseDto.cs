using E_Commerce_Backend.Enums;

namespace E_Commerce_Backend.DTOs.Users.Responses
{
    // GET /users/me
    public class UserProfileResponseDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public UserRole Role { get; set; }
        public string? Avatar { get; set; }
        public List<AddressResponseDto> Addresses { get; set; } = new();
        public DateTime CreatedAt { get; set; }
    }
}
