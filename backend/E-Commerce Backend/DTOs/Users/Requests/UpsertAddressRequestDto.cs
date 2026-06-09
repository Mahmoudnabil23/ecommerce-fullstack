namespace E_Commerce_Backend.DTOs.Users.Requests
{
    // POST /users/me/addresses  |  PUT /users/me/addresses/{id}
    public class UpsertAddressRequestDto
    {
        public string Label { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string? PostalCode { get; set; }
        public bool IsDefault { get; set; } = false;
    }
}
