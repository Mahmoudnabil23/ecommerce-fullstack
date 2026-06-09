using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce_Backend.Models.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Categories");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id)
                .HasDefaultValueSql("NEWSEQUENTIALID()");

            // Source: API Contract GET /categories → name (required)
            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);

            // Source: API Contract GET /categories → slug (required, unique)
            builder.Property(c => c.Slug)
                .IsRequired()
                .HasMaxLength(120);

            // Source: API Contract GET /categories → imageUrl (nullable)
            builder.Property(c => c.ImageUrl)
                .HasMaxLength(500);

            // Source: SRS §6 → soft delete
            builder.Property(c => c.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            // ── Indexes ─────────────────────────────────────────────────────
            builder.HasIndex(c => c.Slug)
                .IsUnique()
                .HasDatabaseName("IX_Categories_Slug");

            builder.HasIndex(c => c.IsDeleted)
                .HasDatabaseName("IX_Categories_IsDeleted");

            // ── Global Query Filter ──────────────────────────────────────
            builder.HasQueryFilter(c => !c.IsDeleted);

            // ── Self-referencing relationship (tree structure) ───────────
            // Source: API Contract GET /categories → children array
            builder.HasOne(c => c.Parent)
                .WithMany(c => c.Children)
                .HasForeignKey(c => c.ParentId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict); // prevent cascade on tree
        }
    }

}
