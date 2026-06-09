using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce_Backend.Models.Configurations
{
    public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
    {
        public void Configure(EntityTypeBuilder<CartItem> builder)
        {
            builder.ToTable("CartItems");

            builder.HasKey(ci => ci.Id);

            builder.Property(ci => ci.Id)
                .HasDefaultValueSql("NEWSEQUENTIALID()");

            // Source: API Contract GET /cart → quantity (required, min 1)
            builder.Property(ci => ci.Quantity)
                .IsRequired();

            // ── Indexes ─────────────────────────────────────────────────────
            // Composite unique: one row per product per cart
            builder.HasIndex(ci => new { ci.CartId, ci.ProductId })
                .IsUnique()
                .HasDatabaseName("IX_CartItems_CartId_ProductId");

            // ── Relationships ────────────────────────────────────────────
            builder.HasOne(ci => ci.Cart)
                .WithMany(c => c.Items)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ci => ci.Product)
                .WithMany(p => p.CartItems)
                .HasForeignKey(ci => ci.ProductId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }

}
