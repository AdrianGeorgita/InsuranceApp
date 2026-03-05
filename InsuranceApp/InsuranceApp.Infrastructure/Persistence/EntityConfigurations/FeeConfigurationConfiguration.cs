using InsuranceApp.Domain.Entities;
using InsuranceApp.Infrastructure.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InsuranceApp.Infrastructure.Persistence.EntityConfigurations;

public class FeeConfigurationConfiguration : IEntityTypeConfiguration<FeeConfiguration>
{
    public void Configure(EntityTypeBuilder<FeeConfiguration> builder)
    {
        builder.HasKey(e => e.Id).HasName("PK__FeeConfi__3214EC073A438B8E");

        builder.Property(e => e.Id).ValueGeneratedNever();
        builder.Property(e => e.CreatedAt)
            .HasPrecision(0)
            .HasDefaultValueSql(DbConstants.SysUtcDateTime);
        builder.Property(e => e.EffectiveFrom).HasPrecision(0);
        builder.Property(e => e.EffectiveTo).HasPrecision(0);
        builder.Property(e => e.Name).HasMaxLength(256);
        builder.Property(e => e.Percentage).HasColumnType("decimal(10, 6)");
        builder.Property(e => e.Type).HasConversion<string>().HasMaxLength(128);
        builder.Property(e => e.UpdatedAt)
            .HasPrecision(0)
            .IsRowVersion()
            .HasDefaultValueSql(DbConstants.SysUtcDateTime);
    }
}