using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce_Backend.Models.Configurations
{
    public class BannerConfiguration : IEntityTypeConfiguration<Banner>
    {
        public void Configure(EntityTypeBuilder<Banner> builder)
        {
            builder.ToTable("Banners");

            builder.HasKey(b => b.Id);

            builder.Property(b => b.Id)
                .HasDefaultValueSql("NEWSEQUENTIALID()");

            // Source: API Contract POST /admin/banners → title (required)
            builder.Property(b => b.Title)
                .IsRequired()
                .HasMaxLength(200);

            // Source: API Contract → imageFile → stored as CDN URL
            builder.Property(b => b.ImageUrl)
                .IsRequired()
                .HasMaxLength(500);

            // Source: API Contract → linkUrl (nullable)
            builder.Property(b => b.LinkUrl)
                .HasMaxLength(500);

            // Source: API Contract → position (int ordering)
            builder.Property(b => b.Position)
                .IsRequired();

            // Source: API Contract → isActive
            builder.Property(b => b.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(b => b.CreatedAt)
                .IsRequired()
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETUTCDATE()");

            // ── Indexes ─────────────────────────────────────────────────────
            builder.HasIndex(b => b.IsActive)
                .HasDatabaseName("IX_Banners_IsActive");

            builder.HasIndex(b => b.Position)
                .HasDatabaseName("IX_Banners_Position");

            // Composite: active banners ordered by position (most common query)
            builder.HasIndex(b => new { b.IsActive, b.Position })
                .HasDatabaseName("IX_Banners_IsActive_Position");
        }
    }

}
