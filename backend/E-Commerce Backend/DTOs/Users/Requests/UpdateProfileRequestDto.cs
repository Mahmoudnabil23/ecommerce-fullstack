namespace E_Commerce_Backend.DTOs.Users.Requests
{

    // PUT /users/me
    public class UpdateProfileRequestDto
    {
        public string? FullName { get; set; }
        public string? Phone { get; set; }
    }
}
