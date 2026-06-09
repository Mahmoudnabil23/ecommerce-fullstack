using E_Commerce_Backend.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce_Backend.Models.Configurations
{
    public class SellerProfileConfiguration : IEntityTypeConfiguration<SellerProfile>
    {
        public void Configure(EntityTypeBuilder<SellerProfile> builder)
        {
            builder.ToTable("SellerProfiles");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.Id)
                .HasDefaultValueSql("NEWSEQUENTIALID()");

            // Source: API Contract POST /sellers/register → storeName (required)
            builder.Property(s => s.StoreName)
                .IsRequired()
                .HasMaxLength(150);

            // Source: API Contract POST /sellers/register → storeDescription (nullable)
            builder.Property(s => s.StoreDescription)
                .HasMaxLength(1000);

            // Source: API Contract POST /sellers/register → taxId (nullable)
            builder.Property(s => s.TaxId)
                .HasMaxLength(50);

            // Source: API Contract PUT /admin/sellers/{id}/verify → verified bool
            builder.Property(s => s.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20)
                .HasDefaultValue(SellerStatus.Pending);

            // Source: API Contract GET /sellers/{id} → public store rating
            builder.Property(s => s.AverageRating)
                .IsRequired()
                .HasColumnType("decimal(3,2)")
                .HasDefaultValue(0m);

            // Source: API Contract POST /sellers/register → bankAccount object
            builder.Property(s => s.BankName)
                .HasMaxLength(100);

            builder.Property(s => s.BankAccountNumber)
                .HasMaxLength(50);

            builder.Property(s => s.BankAccountHolderName)
                .HasMaxLength(150);

            builder.Property(s => s.CreatedAt)
                .IsRequired()
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETUTCDATE()");

            // ── Indexes ─────────────────────────────────────────────────────
            builder.HasIndex(s => s.UserId)
                .IsUnique()
                .HasDatabaseName("IX_SellerProfiles_UserId");

            builder.HasIndex(s => s.Status)
                .HasDatabaseName("IX_SellerProfiles_Status");

            // ── 1-to-1 with ApplicationUser ──────────────────────────────
            builder.HasOne(s => s.User)
                .WithOne(u => u.SellerProfile)
                .HasForeignKey<SellerProfile>(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

}
