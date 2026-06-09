using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce_Backend.Models.Configurations
{
    public class ReviewConfiguration : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            builder.ToTable("Reviews");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Id)
                .HasDefaultValueSql("NEWSEQUENTIALID()");

            // Source: API Contract GET /products/{id}/reviews → rating (1–5)
            builder.Property(r => r.Rating)
                .IsRequired();

            builder.ToTable("Reviews", t =>
                t.HasCheckConstraint("CK_Reviews_Rating", "[Rating] BETWEEN 1 AND 5"));

            // Source: API Contract → title (required)
            builder.Property(r => r.Title)
                .IsRequired()
                .HasMaxLength(150);

            // Source: API Contract → body (required)
            builder.Property(r => r.Body)
                .IsRequired()
                .HasColumnType("nvarchar(max)");

            // Source: API Contract → images array — stored as JSON
            builder.Property(r => r.ImagesJson)
                .HasColumnType("nvarchar(max)");

            // Source: API Contract → isVerifiedPurchase
            builder.Property(r => r.IsVerifiedPurchase)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(r => r.CreatedAt)
                .IsRequired()
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETUTCDATE()");

            // ── Indexes ─────────────────────────────────────────────────────
            // One review per user per product (enforced by 409 in API Contract)
            builder.HasIndex(r => new { r.UserId, r.ProductId })
                .IsUnique()
                .HasDatabaseName("IX_Reviews_UserId_ProductId");

            builder.HasIndex(r => r.ProductId)
                .HasDatabaseName("IX_Reviews_ProductId");

            // ── Relationships ────────────────────────────────────────────
            builder.HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(r => r.Product)
                .WithMany(p => p.Reviews)
                .HasForeignKey(r => r.ProductId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }

}
