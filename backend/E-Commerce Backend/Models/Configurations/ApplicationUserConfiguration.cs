using E_Commerce_Backend.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce_Backend.Models.Configurations
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.ToTable("Users");

            //builder.Property(u => u.Id)
            //    .HasDefaultValueSql("NEWSEQUENTIALID()");

            // Source: API Contract GET /users/me → fullName (required)
            builder.Property(u => u.FullName)
                .IsRequired()
                .HasMaxLength(150);

            // Source: API Contract GET /users/me → avatar (nullable URL)
            builder.Property(u => u.AvatarUrl)
                .HasMaxLength(500);



            // Source: API Contract PUT /admin/users/{id}/status
            builder.Property(u => u.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20)
                .HasDefaultValue(UserStatus.Active);


           

            builder.Property(u => u.CreatedAt)
                .IsRequired()
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETUTCDATE()");

            // Source: SRS §6 Admin → soft delete
            builder.Property(u => u.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(u => u.DeletedAt)
                .HasColumnType("datetime2");



            builder.HasIndex(u => u.IsDeleted)
                .HasDatabaseName("IX_Users_IsDeleted");

            // ── Global Query Filter — exclude soft-deleted users ─────────
            builder.HasQueryFilter(u => !u.IsDeleted);

            // ── Relationships ────────────────────────────────────────────
            // Addresses: 1-to-many — configured on Address side
            // Orders: 1-to-many — configured on Order side
            // WishlistItems, Reviews, Notifications — configured on their side
            // Cart: 1-to-1 — configured on Cart side
            // SellerProfile: 1-to-1 — configured on SellerProfile side
        }
    }

}
