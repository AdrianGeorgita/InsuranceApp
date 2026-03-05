using InsuranceApp.Domain.Entities;
using InsuranceApp.Infrastructure.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InsuranceApp.Infrastructure.Persistence.EntityConfigurations;

public class CityConfiguration : IEntityTypeConfiguration<City>
{
    public void Configure(EntityTypeBuilder<City> builder)
    {
        builder.HasKey(e => e.Id).HasName("PK__Cities__3214EC0788A2A5E6");

        builder.HasIndex(e => new { e.CountyId, e.Name }, "UQ_Cities_CountyId_Name").IsUnique();

        builder.Property(e => e.Id).ValueGeneratedNever();
        builder.Property(e => e.CreatedAt)
            .HasPrecision(0)
            .HasDefaultValueSql(DbConstants.SysUtcDateTime);
        builder.Property(e => e.Name).HasMaxLength(255);
        builder.Property(e => e.UpdatedAt)
            .HasPrecision(0)
            .IsRowVersion()
            .HasDefaultValueSql(DbConstants.SysUtcDateTime);

        builder.HasOne(d => d.County).WithMany(p => p.Cities)
            .HasForeignKey(d => d.CountyId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Cities_Counties");
    }
}