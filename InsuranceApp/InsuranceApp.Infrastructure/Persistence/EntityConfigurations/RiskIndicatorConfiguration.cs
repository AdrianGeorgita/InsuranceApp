using InsuranceApp.Domain.Entities;
using InsuranceApp.Infrastructure.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InsuranceApp.Infrastructure.Persistence.EntityConfigurations;

public class RiskIndicatorConfiguration : IEntityTypeConfiguration<RiskIndicator>
{
    public void Configure(EntityTypeBuilder<RiskIndicator> builder)
    {
        builder.HasKey(e => e.Id).HasName("PK__RiskIndi__3214EC0705F8A6BF");

        builder.HasIndex(e => e.Name, "UQ_RiskIndicators_Name").IsUnique();

        builder.Property(e => e.CreatedAt)
            .HasPrecision(0)
            .HasDefaultValueSql(DbConstants.SysUtcDateTime);
        builder.Property(e => e.IsActive).HasDefaultValue(true);
        builder.Property(e => e.Name).HasMaxLength(255);
        builder.Property(e => e.UpdatedAt)
            .HasPrecision(0)
            .IsRowVersion()
            .HasDefaultValueSql(DbConstants.SysUtcDateTime);
    }
}