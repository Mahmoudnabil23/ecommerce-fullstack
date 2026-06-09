using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce_Backend.Models.Configurations
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.ToTable("OrderItems");

            builder.HasKey(oi => oi.Id);

            builder.Property(oi => oi.Id)
                .HasDefaultValueSql("NEWSEQUENTIALID()");

            // Source: API Contract GET /orders/{id} → qty
            builder.Property(oi => oi.Quantity)
                .IsRequired();

            // Source: API Contract GET /orders/{id} → unitPrice
            // Snapshot — price at the moment of order placement
            builder.Property(oi => oi.UnitPrice)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            // Source: API Contract GET /orders/{id} → name (snapshot)
            // Product name at time of order — not a live lookup
            builder.Property(oi => oi.ProductName)
                .IsRequired()
                .HasMaxLength(200);

            // ── Indexes ─────────────────────────────────────────────────────
            builder.HasIndex(oi => oi.OrderId)
                .HasDatabaseName("IX_OrderItems_OrderId");

            builder.HasIndex(oi => oi.ProductId)
                .HasDatabaseName("IX_OrderItems_ProductId");

            // ── Relationships ────────────────────────────────────────────
            builder.HasOne(oi => oi.Order)
                .WithMany(o => o.Items)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(oi => oi.Product)
                .WithMany(p => p.OrderItems)
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }

}
