using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce_Backend.Models.Configurations
{
    public class CartConfiguration : IEntityTypeConfiguration<Cart>
    {
        public void Configure(EntityTypeBuilder<Cart> builder)
        {
            builder.ToTable("Carts");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id)
                .HasDefaultValueSql("NEWSEQUENTIALID()");

            // Source: API Contract Headers → X-Guest-Session: {uuid}
            builder.Property(c => c.GuestSessionId)
                .HasMaxLength(100);

            builder.Property(c => c.CreatedAt)
                .IsRequired()
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(c => c.UpdatedAt)
                .IsRequired()
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETUTCDATE()");

            // ── Indexes ─────────────────────────────────────────────────────
            builder.HasIndex(c => c.UserId)
                .IsUnique()
                .HasFilter("[UserId] IS NOT NULL") // partial index — only for authenticated carts
                .HasDatabaseName("IX_Carts_UserId");

            builder.HasIndex(c => c.GuestSessionId)
                .HasFilter("[GuestSessionId] IS NOT NULL")
                .HasDatabaseName("IX_Carts_GuestSessionId");

            // ── Relationships ────────────────────────────────────────────
            // Source: SRS §3 → guest checkout — UserId nullable
            builder.HasOne(c => c.User)
                .WithOne(u => u.Cart)
                .HasForeignKey<Cart>(c => c.UserId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);

            // Source: API Contract POST /cart/promo → applied promo code (nullable)
            builder.HasOne(c => c.AppliedPromoCode)
                .WithMany()
                .HasForeignKey(c => c.AppliedPromoCodeId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }

}
