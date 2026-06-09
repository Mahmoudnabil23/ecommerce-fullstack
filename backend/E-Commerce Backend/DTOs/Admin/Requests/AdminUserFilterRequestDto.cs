using E_Commerce_Backend.Enums;

namespace E_Commerce_Backend.DTOs.Admin.Requests
{
    // GET /admin/users — query params
    public class AdminUserFilterRequestDto
    {
        public UserRole? Role { get; set; }
        public UserStatus? Status { get; set; }
        public string? Search { get; set; }
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 20;
    }
}
