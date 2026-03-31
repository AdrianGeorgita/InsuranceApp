using InsuranceApp.Domain.Entities;
using InsuranceApp.Infrastructure.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InsuranceApp.Infrastructure.Persistence.EntityConfigurations;

public class CurrencyConfiguration : IEntityTypeConfiguration<Currency>
{
    public void Configure(EntityTypeBuilder<Currency> builder)
    {
        builder.HasKey(e => e.Code).HasName("PK__Currenci__A25C5AA6E4DC565C");

        builder.Property(e => e.Code).HasMaxLength(3);
        builder.Property(e => e.CreatedAt)
            .HasPrecision(0)
            .HasDefaultValueSql(DbConstants.SysUtcDateTime);
        builder.Property(e => e.ExchangeRateToBase).HasColumnType("decimal(10, 6)");
        builder.Property(e => e.IsActive).HasDefaultValue(true);
        builder.Property(e => e.Name).HasMaxLength(256);
        builder.Property(e => e.UpdatedAt)
            .HasPrecision(0)
            .IsRowVersion()
            .HasDefaultValueSql(DbConstants.SysUtcDateTime);
    }
}