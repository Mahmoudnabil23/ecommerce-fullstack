namespace E_Commerce_Backend.DTOs.Sellers.Requests
{

    // POST /sellers/register
    public class SellerRegistrationRequestDto
    {
        public string StoreName { get; set; } = string.Empty;
        public string? StoreDescription { get; set; }
        public string? TaxId { get; set; }
        public BankAccountDto BankAccount { get; set; } = new();
    }
}
