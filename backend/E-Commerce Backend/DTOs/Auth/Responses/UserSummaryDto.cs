using E_Commerce_Backend.Enums;

namespace E_Commerce_Backend.DTOs.Auth.Responses
{
    public class UserSummaryDto
    {
        public string Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new List<string>();
    }
}
