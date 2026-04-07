using InsuranceApp.Domain.Entities;
using InsuranceApp.Infrastructure.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InsuranceApp.Infrastructure.Persistence.EntityConfigurations;

public class PolicyAuditLogConfiguration : IEntityTypeConfiguration<PolicyAuditLog>
{
    public void Configure(EntityTypeBuilder<PolicyAuditLog> builder)
    {

        builder.HasKey(e => e.Id).HasName("PK__PolicyAu__3214EC07C8179D09");

        builder.HasIndex(e => new { e.BrokerId, e.OccurredAt }, "IX_PolicyAuditLogs_Broker").IsDescending(false, true);

        builder.HasIndex(e => new { e.PolicyNumber, e.OccurredAt }, "IX_PolicyAuditLogs_PolicyNumber").IsDescending(false, true);

        builder.Property(e => e.Id).ValueGeneratedNever();
        builder.Property(e => e.ColumnName).HasMaxLength(128);
        builder.Property(e => e.OccurredAt)
            .HasPrecision(0)
            .HasDefaultValueSql(DbConstants.SysUtcDateTime);
        builder.Property(e => e.PolicyNumber).HasMaxLength(256);
    }
}