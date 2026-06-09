using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce_Backend.Models.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .HasDefaultValueSql("NEWSEQUENTIALID()");

            // Source: API Contract GET /products → name (required)
            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(200);

            // Source: API Contract GET /products → slug (required, unique)
            builder.Property(p => p.Slug)
                .HasMaxLength(250);

            // Source: SRS §2 → descriptions (required)
            builder.Property(p => p.Description)
                .IsRequired()
                .HasColumnType("nvarchar(max)");

            // Source: API Contract GET /products → price
            builder.Property(p => p.Price)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            // Source: API Contract GET /products → discountedPrice (nullable)
            builder.Property(p => p.DiscountedPrice)
                .HasColumnType("decimal(18,2)");

            // Source: SRS §2 → stock availability
            builder.Property(p => p.Stock)
                .IsRequired()
                .HasDefaultValue(0);

            // Source: API Contract POST /products → specs as JSON string
            builder.Property(p => p.SpecsJson)
                .HasColumnType("nvarchar(max)");

            builder.Property(p => p.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(p => p.DeletedAt)
                .HasColumnType("datetime2");

            builder.Property(p => p.CreatedAt)
                .IsRequired()
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETUTCDATE()");

            // ── Indexes ─────────────────────────────────────────────────────
            builder.HasIndex(p => p.Slug)
                .IsUnique()
                .HasDatabaseName("IX_Products_Slug");

            builder.HasIndex(p => p.CategoryId)
                .HasDatabaseName("IX_Products_CategoryId");

            builder.HasIndex(p => p.SellerId)
                .HasDatabaseName("IX_Products_SellerId");

            builder.HasIndex(p => p.Price)
                .HasDatabaseName("IX_Products_Price");

            builder.HasIndex(p => p.IsDeleted)
                .HasDatabaseName("IX_Products_IsDeleted");

            // Composite: filter by category + soft delete (common query pattern)
            builder.HasIndex(p => new { p.CategoryId, p.IsDeleted })
                .HasDatabaseName("IX_Products_CategoryId_IsDeleted");

            // ── Global Query Filter ──────────────────────────────────────
            builder.HasQueryFilter(p => !p.IsDeleted);

            // ── Relationships ────────────────────────────────────────────
            // Source: API Contract GET /products → category object
            builder.HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict); // product survives category soft-delete

            // Source: API Contract GET /products → seller object
            builder.HasOne(p => p.Seller)
                .WithMany(s => s.Products)
                .HasForeignKey(p => p.SellerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

}
