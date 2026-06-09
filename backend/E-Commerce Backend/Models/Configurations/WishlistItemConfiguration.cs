using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce_Backend.Models.Configurations
{
    public class WishlistItemConfiguration : IEntityTypeConfiguration<WishlistItem>
    {
        public void Configure(EntityTypeBuilder<WishlistItem> builder)
        {
            builder.ToTable("WishlistItems");

            builder.HasKey(w => w.Id);

            builder.Property(w => w.Id)
                .HasDefaultValueSql("NEWSEQUENTIALID()");

            builder.Property(w => w.AddedAt)
                .IsRequired()
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETUTCDATE()");

            // ── Indexes ─────────────────────────────────────────────────────
            // One wishlist entry per user per product (409 check in API Contract)
            builder.HasIndex(w => new { w.UserId, w.ProductId })
                .IsUnique()
                .HasDatabaseName("IX_WishlistItems_UserId_ProductId");

            builder.HasIndex(w => w.UserId)
                .HasDatabaseName("IX_WishlistItems_UserId");

            // ── Relationships ────────────────────────────────────────────
            builder.HasOne(w => w.User)
                .WithMany(u => u.WishlistItems)
                .HasForeignKey(w => w.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(w => w.Product)
                .WithMany(p => p.WishlistItems)
                .HasForeignKey(w => w.ProductId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }

}
