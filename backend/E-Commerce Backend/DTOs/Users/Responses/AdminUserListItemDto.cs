using E_Commerce_Backend.Enums;

namespace E_Commerce_Backend.DTOs.Users.Responses
{
    // GET /admin/users list item
    public class AdminUserListItemDto
    {
        public string Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<string> Roles { get; set; }
        public UserStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
