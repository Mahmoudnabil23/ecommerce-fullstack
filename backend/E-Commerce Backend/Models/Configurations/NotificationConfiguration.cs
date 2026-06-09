using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce_Backend.Models.Configurations
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.ToTable("Notifications");

            builder.HasKey(n => n.Id);

            builder.Property(n => n.Id)
                .HasDefaultValueSql("NEWSEQUENTIALID()");

            // Source: API Contract GET /notifications → type (string — enum not defined in SRS)
            builder.Property(n => n.Type)
                .IsRequired()
                .HasMaxLength(50);

            // Source: API Contract → message content
            builder.Property(n => n.Message)
                .IsRequired()
                .HasMaxLength(500);

            // Source: API Contract GET /notifications?read=false
            builder.Property(n => n.IsRead)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(n => n.CreatedAt)
                .IsRequired()
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETUTCDATE()");

            // ── Indexes ─────────────────────────────────────────────────────
            builder.HasIndex(n => n.UserId)
                .HasDatabaseName("IX_Notifications_UserId");

            // Composite: most common query — user's unread notifications
            builder.HasIndex(n => new { n.UserId, n.IsRead })
                .HasDatabaseName("IX_Notifications_UserId_IsRead");

            builder.HasIndex(n => n.CreatedAt)
                .HasDatabaseName("IX_Notifications_CreatedAt");

            // ── Relationships ────────────────────────────────────────────
            builder.HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }

}
