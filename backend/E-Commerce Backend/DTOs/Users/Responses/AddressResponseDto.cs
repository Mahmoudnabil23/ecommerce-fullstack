namespace E_Commerce_Backend.DTOs.Users.Responses
{
    // Address item (used in GET /users/me and standalone address endpoints)
    public class AddressResponseDto
    {
        public Guid Id { get; set; }
        public string Label { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string? PostalCode { get; set; }
        public bool IsDefault { get; set; }
    }
}
