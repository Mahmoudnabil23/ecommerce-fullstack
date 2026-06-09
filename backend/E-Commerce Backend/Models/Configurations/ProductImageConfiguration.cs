using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce_Backend.Models.Configurations
{
    public class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
    {
        public void Configure(EntityTypeBuilder<ProductImage> builder)
        {
            builder.ToTable("ProductImages");

            builder.HasKey(pi => pi.Id);

            builder.Property(pi => pi.Id)
                .HasDefaultValueSql("NEWSEQUENTIALID()");

            // Source: API Contract GET /products → images array (URL strings)
            builder.Property(pi => pi.ImageUrl)
                .IsRequired()
                .HasMaxLength(500);

            // Display order for multi-image carousel
            builder.Property(pi => pi.DisplayOrder)
                .IsRequired()
                .HasDefaultValue(0);

            // ── Indexes ─────────────────────────────────────────────────────
            builder.HasIndex(pi => pi.ProductId)
                .HasDatabaseName("IX_ProductImages_ProductId");

            // ── Relationships ────────────────────────────────────────────
            builder.HasOne(pi => pi.Product)
                .WithMany(p => p.Images)
                .HasForeignKey(pi => pi.ProductId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }

}
