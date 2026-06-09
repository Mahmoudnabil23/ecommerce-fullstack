using E_Commerce_Backend.Enums;

namespace E_Commerce_Backend.DTOs.Admin.Requests
{
    // PUT /admin/users/{id}/status
    public class UpdateUserStatusRequestDto
    {
        public UserStatus Status { get; set; }
        public string? Reason { get; set; }
    }
}
