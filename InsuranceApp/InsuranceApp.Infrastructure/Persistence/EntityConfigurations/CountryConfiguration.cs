using InsuranceApp.Domain.Entities;
using InsuranceApp.Infrastructure.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InsuranceApp.Infrastructure.Persistence.EntityConfigurations;

public class CountryConfiguration : IEntityTypeConfiguration<Country>
{
    public void Configure(EntityTypeBuilder<Country> builder)
    {
        builder.HasKey(e => e.Id).HasName("PK__Countrie__3214EC07AAC5CB0A");

        builder.HasIndex(e => e.Iso2, "UQ_Countries_ISO2").IsUnique();

        builder.HasIndex(e => e.Iso3, "UQ_Countries_ISO3").IsUnique();

        builder.Property(e => e.Id).ValueGeneratedNever();
        builder.Property(e => e.CreatedAt)
            .HasPrecision(0)
            .HasDefaultValueSql(DbConstants.SysUtcDateTime);
        builder.Property(e => e.Iso2)
            .HasMaxLength(2)
            .HasColumnName("ISO2");
        builder.Property(e => e.Iso3)
            .HasMaxLength(3)
            .HasColumnName("ISO3");
        builder.Property(e => e.Name).HasMaxLength(255);
        builder.Property(e => e.UpdatedAt)
            .HasPrecision(0)
            .IsRowVersion()
            .HasDefaultValueSql(DbConstants.SysUtcDateTime);
    }
}