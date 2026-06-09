using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce_Backend.Models.Configurations
{
    public class PromoCodeConfiguration : IEntityTypeConfiguration<PromoCode>
    {
        public void Configure(EntityTypeBuilder<PromoCode> builder)
        {
            builder.ToTable("PromoCodes");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .HasDefaultValueSql("NEWSEQUENTIALID()");

            // Source: API Contract POST /admin/promo-codes → code (required, unique)
            builder.Property(p => p.Code)
                .IsRequired()
                .HasMaxLength(50);

            // Source: API Contract → type: "percentage" | "fixed"
            builder.Property(p => p.Type)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);

            // Source: API Contract → value
            builder.Property(p => p.Value)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            // Source: API Contract → minOrderAmount
            builder.Property(p => p.MinOrderAmount)
                .IsRequired()
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(0m);

            // Source: API Contract → usageLimit
            builder.Property(p => p.UsageLimit)
                .IsRequired();

            builder.Property(p => p.UsageCount)
                .IsRequired()
                .HasDefaultValue(0);

            // Source: API Contract → expiresAt
            builder.Property(p => p.ExpiresAt)
                .IsRequired()
                .HasColumnType("datetime2");

            // Source: API Contract → applicableCategories as JSON
            builder.Property(p => p.ApplicableCategoriesJson)
                .HasColumnType("nvarchar(max)");

            builder.Property(p => p.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(p => p.CreatedAt)
                .IsRequired()
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETUTCDATE()");

            // ── Indexes ─────────────────────────────────────────────────────
            builder.HasIndex(p => p.Code)
                .IsUnique()
                .HasDatabaseName("IX_PromoCodes_Code");

            builder.HasIndex(p => p.IsActive)
                .HasDatabaseName("IX_PromoCodes_IsActive");

            builder.HasIndex(p => p.ExpiresAt)
                .HasDatabaseName("IX_PromoCodes_ExpiresAt");
        }
    }

}
