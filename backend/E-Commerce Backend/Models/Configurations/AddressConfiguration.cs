using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce_Backend.Models.Configurations
{
    public class AddressConfiguration : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.ToTable("Addresses");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id)
                .HasDefaultValueSql("NEWSEQUENTIALID()");

            // Source: API Contract POST /users/me/addresses → label (required)
            builder.Property(a => a.Label)
                .IsRequired()
                .HasMaxLength(50);

            // Source: API Contract → street (required)
            builder.Property(a => a.Street)
                .IsRequired()
                .HasMaxLength(300);

            // Source: API Contract → city (required)
            builder.Property(a => a.City)
                .IsRequired()
                .HasMaxLength(100);

            // Source: API Contract → postalCode (nullable)
            builder.Property(a => a.PostalCode)
                .HasMaxLength(20);

            builder.Property(a => a.IsDefault)
                .IsRequired()
                .HasDefaultValue(false);

            // ── Indexes ─────────────────────────────────────────────────────
            builder.HasIndex(a => a.UserId)
                .HasDatabaseName("IX_Addresses_UserId");

            builder.HasIndex(a => new { a.UserId, a.IsDefault })
                .HasDatabaseName("IX_Addresses_UserId_IsDefault");

            // ── Relationships ────────────────────────────────────────────
            builder.HasOne(a => a.User)
                .WithMany(u => u.Addresses)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }

}
