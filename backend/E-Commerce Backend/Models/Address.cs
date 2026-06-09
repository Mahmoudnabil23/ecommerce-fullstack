namespace E_Commerce_Backend.Models
{
    public class Address
    {
        public Guid Id { get; set; }

        // Source: API Contract → label field
        public string Label { get; set; } = string.Empty;

        // Source: API Contract → street field
        public string Street { get; set; } = string.Empty;

        // Source: API Contract → city field
        public string City { get; set; } = string.Empty;

        // Source: API Contract POST /users/me/addresses → postalCode
        public string? PostalCode { get; set; }

        // Source: API Contract → isDefault flag
        public bool IsDefault { get; set; } = false;

        // ── Foreign Key ─────────────────────────────────────────────────────
        public string UserId { get; set; }
        public ApplicationUser User { get; set; } = null!;
    }

}
