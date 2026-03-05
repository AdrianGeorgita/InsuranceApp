using InsuranceApp.Domain.Entities;
using InsuranceApp.Domain.Enums;
using InsuranceApp.Infrastructure.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InsuranceApp.Infrastructure.Persistence.EntityConfigurations;

public class BrokerConfiguration : IEntityTypeConfiguration<Broker>
{
    public void Configure(EntityTypeBuilder<Broker> builder)
    {
        builder.HasKey(e => e.Id).HasName("PK__Brokers__3214EC07BEE2B83E");

        builder.HasIndex(e => e.Code, "UQ_Brokers_Code").IsUnique();

        builder.HasIndex(e => e.Email, "UQ_Brokers_Email").IsUnique();

        builder.HasIndex(e => e.Phone, "UQ_Brokers_Phone").IsUnique();

        builder.Property(e => e.Id).ValueGeneratedNever();
        builder.Property(e => e.Code).HasMaxLength(128);
        builder.Property(e => e.CommissionPercentage)
            .HasDefaultValue(0.0m)
            .HasColumnType("decimal(10, 6)");
        builder.Property(e => e.CreatedAt)
            .HasPrecision(0)
            .HasDefaultValueSql(DbConstants.SysUtcDateTime);
        builder.Property(e => e.Email).HasMaxLength(256);
        builder.Property(e => e.Name).HasMaxLength(256);
        builder.Property(e => e.Phone).HasMaxLength(15);
        builder.Property(e => e.UpdatedAt)
            .HasPrecision(0)
            .IsRowVersion()
            .HasDefaultValueSql(DbConstants.SysUtcDateTime);
        builder.Property(e => e.Status)
            .HasConversion(
                v => v == BrokerStatus.Active,
                v => v ? BrokerStatus.Active : BrokerStatus.Inactive
            );
    }
}