using InsuranceApp.Domain.Entities;
using InsuranceApp.Infrastructure.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InsuranceApp.Infrastructure.Persistence.EntityConfigurations;

public class RiskFactorConfigurationConfiguration : IEntityTypeConfiguration<RiskFactorConfiguration>
{
    public void Configure(EntityTypeBuilder<RiskFactorConfiguration> builder)
    {
        builder.HasKey(e => e.Id).HasName("PK__RiskFact__3214EC070003BBFA");

        builder.HasIndex(e => new { e.Level, e.ReferenceId, e.BuildingType }, "UQ_RiskFactorConfigurations_Level_ReferenceId_BuildingType").IsUnique();

        builder.Property(e => e.Id).ValueGeneratedNever();
        builder.Property(e => e.AdjustmentPercentage).HasColumnType("decimal(10, 6)");
        builder.Property(e => e.CreatedAt)
            .HasPrecision(0)
            .HasDefaultValueSql(DbConstants.SysUtcDateTime);
        builder.Property(e => e.IsActive).HasDefaultValue(true);
        builder.Property(e => e.Level).HasConversion<string>().HasMaxLength(64);
        builder.Property(e => e.BuildingType).HasConversion<string>().HasMaxLength(50);
        builder.Property(e => e.UpdatedAt)
            .HasPrecision(0)
            .IsRowVersion()
            .HasDefaultValueSql(DbConstants.SysUtcDateTime);
    }
}