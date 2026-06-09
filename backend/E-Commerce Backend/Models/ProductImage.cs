namespace E_Commerce_Backend.Models
{
    public class ProductImage
    {
        public Guid Id { get; set; }

        // Source: API Contract GET /products → images array (URL strings)
        public string ImageUrl { get; set; } = string.Empty;

        // Display order — required by DELETE /products/{id}/images/{imageId} existence
        // NOT DEFINED IN SRS/API CONTRACT as explicit field; required by multi-image model
        public int DisplayOrder { get; set; }

        // ── Foreign Key ─────────────────────────────────────────────────────
        public Guid ProductId { get; set; }
        public Product Product { get; set; } = null!;
    }


}

