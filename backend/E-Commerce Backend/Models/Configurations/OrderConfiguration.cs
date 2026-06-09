using E_Commerce_Backend.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce_Backend.Models.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders");

            builder.HasKey(o => o.Id);

            builder.Property(o => o.Id)
                .HasDefaultValueSql("NEWSEQUENTIALID()");

            // Source: API Contract → orderNumber (e.g., "ORD-20240115-00042")
            builder.Property(o => o.OrderNumber)
                .IsRequired()
                .HasMaxLength(50);

            // Source: API Contract → status flow
            builder.Property(o => o.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(30)
                .HasDefaultValue(OrderStatus.Pending);

            // Source: API Contract POST /orders → paymentMethod
            builder.Property(o => o.PaymentMethod)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(30);

            // Source: API Contract GET /orders/{id} → paymentStatus
            builder.Property(o => o.PaymentStatus)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20)
                .HasDefaultValue(PaymentStatus.Pending);

            // Source: API Contract GET /orders/{id} → trackingNumber (nullable)
            builder.Property(o => o.TrackingNumber)
                .HasMaxLength(100);

            // Source: API Contract POST /orders → notes (nullable)
            builder.Property(o => o.Notes)
                .HasMaxLength(500);

            // Source: API Contract GET /orders/{id} → total
            builder.Property(o => o.Total)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            // Source: API Contract → estimatedDelivery (nullable)
            builder.Property(o => o.EstimatedDelivery)
                .HasColumnType("datetime2");

            builder.Property(o => o.CreatedAt)
                .IsRequired()
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETUTCDATE()");

            // ── Shipping Address Snapshot ────────────────────────────────
            // Source: API Contract GET /orders/{id} → address object
            // Stored as snapshot — not FK — so address changes don't affect past orders
            builder.Property(o => o.ShippingStreet)
                .IsRequired()
                .HasMaxLength(300);

            builder.Property(o => o.ShippingCity)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(o => o.ShippingPostalCode)
                .HasMaxLength(20);

            // Source: API Contract guest checkout → guest email
            builder.Property(o => o.GuestEmail)
                .HasMaxLength(200);

            // ── Indexes ─────────────────────────────────────────────────────
            builder.HasIndex(o => o.OrderNumber)
                .IsUnique()
                .HasDatabaseName("IX_Orders_OrderNumber");

            builder.HasIndex(o => o.UserId)
                .HasDatabaseName("IX_Orders_UserId");

            builder.HasIndex(o => o.Status)
                .HasDatabaseName("IX_Orders_Status");

            builder.HasIndex(o => o.CreatedAt)
                .HasDatabaseName("IX_Orders_CreatedAt");

            builder.HasIndex(o => o.PaymentStatus)
                .HasDatabaseName("IX_Orders_PaymentStatus");

            // ── Relationships ────────────────────────────────────────────
            // UserId nullable — guest checkout allowed (SRS §3)
            builder.HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);

            // Source: API Contract POST /orders → promoCode applied
            builder.HasOne(o => o.PromoCode)
                .WithMany()
                .HasForeignKey(o => o.PromoCodeId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }

}
