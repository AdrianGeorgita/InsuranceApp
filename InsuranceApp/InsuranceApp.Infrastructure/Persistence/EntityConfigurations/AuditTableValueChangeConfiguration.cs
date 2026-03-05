using InsuranceApp.Domain.Entities;
using InsuranceApp.Infrastructure.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InsuranceApp.Infrastructure.Persistence.EntityConfigurations;

public class AuditTableValueChangeConfiguration : IEntityTypeConfiguration<AuditTableValueChange>
{
    public void Configure(EntityTypeBuilder<AuditTableValueChange> builder)
    {
        builder.HasKey(e => e.Id).HasName("PK__AuditTab__3214EC0751523691");

        builder.HasIndex(e => new { e.EventId, e.UpdatedAt }, "IX_AuditTableValueChanges_EventId").IsDescending(false, true);

        builder.HasIndex(e => new { e.TableName, e.UpdatedAt }, "IX_AuditTableValueChanges_TableName").IsDescending(false, true);

        builder.HasIndex(e => new { e.UserId, e.UpdatedAt }, "IX_AuditTableValueChanges_User").IsDescending(false, true);

        builder.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
        builder.Property(e => e.ColumnName).HasMaxLength(128);
        builder.Property(e => e.RowId).HasMaxLength(256);
        builder.Property(e => e.Reason).HasMaxLength(512);
        builder.Property(e => e.TableName).HasMaxLength(128);
        builder.Property(e => e.UpdatedAt)
            .HasPrecision(0)
            .IsRowVersion()
            .HasDefaultValueSql(DbConstants.SysUtcDateTime);
    }
}