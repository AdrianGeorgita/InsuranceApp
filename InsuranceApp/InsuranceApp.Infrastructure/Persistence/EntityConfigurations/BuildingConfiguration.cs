using InsuranceApp.Domain.Entities;
using InsuranceApp.Infrastructure.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InsuranceApp.Infrastructure.Persistence.EntityConfigurations;

public class BuildingConfiguration : IEntityTypeConfiguration<Building>
{
    public void Configure(EntityTypeBuilder<Building> builder)
    {
        builder.HasKey(e => e.Id).HasName("PK__Building__3214EC075A47808A");

        builder.HasQueryFilter(x => x.IsDeleted == false);

        builder.HasIndex(e => e.Id, "IX_Buildings_Active").HasFilter("([IsDeleted]=(0))");

        builder.Property(e => e.Id).ValueGeneratedNever();
        builder.Property(e => e.Address).HasMaxLength(255);
        builder.Property(e => e.BuildingType).HasMaxLength(50);
        builder.Property(e => e.CreatedAt)
            .HasPrecision(0)
            .HasDefaultValueSql(DbConstants.SysUtcDateTime);
        builder.Property(e => e.InsuredValue).HasColumnType("decimal(18, 0)");
        builder.Property(e => e.SurfaceArea).HasColumnType("decimal(6, 2)");
        builder.Property(e => e.UpdatedAt)
            .HasPrecision(0)
            .IsRowVersion()
            .HasDefaultValueSql(DbConstants.SysUtcDateTime);

        builder.HasOne(d => d.City).WithMany(p => p.Buildings)
            .HasForeignKey(d => d.CityId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Buildings_City");

        builder.HasOne(d => d.Owner).WithMany(p => p.Buildings)
            .HasForeignKey(d => d.OwnerId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Buildings_Owner");

        builder.HasMany(d => d.RiskIndicators).WithMany(p => p.Buildings)
            .UsingEntity<Dictionary<string, object>>(
                "BuildingRiskIndicator",
                r => r.HasOne<RiskIndicator>().WithMany()
                    .HasForeignKey("RiskIndicatorId")
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BuildingRiskIndicators_RiskIndicators"),
                l => l.HasOne<Building>().WithMany()
                    .HasForeignKey("BuildingId")
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BuildingRiskIndicators_Buildings"),
                j =>
                {
                    j.HasKey("BuildingId", "RiskIndicatorId");
                    j.ToTable("BuildingRiskIndicators");
                });
    }
}