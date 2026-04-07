using InsuranceApp.Domain.Entities;
using InsuranceApp.Infrastructure.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InsuranceApp.Infrastructure.Persistence.EntityConfigurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.HasKey(e => e.Id).HasName("PK__AuditLog__3214EC079AFFBC76");

        builder.HasIndex(e => new { e.EventId, e.OccurredAt }, "IX_AuditLogs_EventId").IsDescending(false, true);

        builder.HasIndex(e => new { e.TableName, e.OccurredAt }, "IX_AuditLogs_TableName").IsDescending(false, true);

        builder.HasIndex(e => new { e.UserId, e.OccurredAt }, "IX_AuditLogs_User").IsDescending(false, true);

        builder.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
        builder.Property(e => e.Action).HasMaxLength(128);
        builder.Property(e => e.ColumnName).HasMaxLength(128);
        builder.Property(e => e.OccurredAt)
            .HasPrecision(0)
            .HasDefaultValueSql(DbConstants.SysUtcDateTime);
        builder.Property(e => e.RowId).HasMaxLength(256);
        builder.Property(e => e.TableName).HasMaxLength(128);
    }
}