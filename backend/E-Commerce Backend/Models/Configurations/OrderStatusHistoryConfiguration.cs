using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce_Backend.Models.Configurations
{
    public class OrderStatusHistoryConfiguration : IEntityTypeConfiguration<OrderStatusHistory>
    {
        public void Configure(EntityTypeBuilder<OrderStatusHistory> builder)
        {
            builder.ToTable("OrderStatusHistories");

            builder.HasKey(h => h.Id);

            builder.Property(h => h.Id)
                .HasDefaultValueSql("NEWSEQUENTIALID()");

            // Source: API Contract GET /orders/{id} → statusHistory[].status
            builder.Property(h => h.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(30);

            // Source: API Contract GET /orders/{id} → statusHistory[].timestamp
            builder.Property(h => h.Timestamp)
                .IsRequired()
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETUTCDATE()");

            // ── Indexes ─────────────────────────────────────────────────────
            builder.HasIndex(h => h.OrderId)
                .HasDatabaseName("IX_OrderStatusHistories_OrderId");

            // ── Relationships ────────────────────────────────────────────
            builder.HasOne(h => h.Order)
                .WithMany(o => o.StatusHistory)
                .HasForeignKey(h => h.OrderId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }

}
