namespace E_Commerce_Backend.Models
{
    public class Category
    {
        public Guid Id { get; set; }

        // Source: API Contract GET /categories → name
        public string Name { get; set; } = string.Empty;

        // Source: API Contract GET /categories → slug
        public string Slug { get; set; } = string.Empty;

        // Source: API Contract GET /categories → imageUrl
        public string? ImageUrl { get; set; }

        // Source: API Contract GET /categories → parentId (tree structure / children)
        public Guid? ParentId { get; set; }
        public Category? Parent { get; set; }
        public ICollection<Category> Children { get; set; } = new List<Category>();

        // Source: SRS §6 → soft delete
        public bool IsDeleted { get; set; } = false;

        // ── Navigation ──────────────────────────────────────────────────────
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }

}
