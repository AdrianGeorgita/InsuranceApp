using InsuranceApp.Domain.Entities;
using InsuranceApp.Infrastructure.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InsuranceApp.Infrastructure.Persistence.EntityConfigurations;

public class PolicyConfiguration : IEntityTypeConfiguration<Policy>
{
    public void Configure(EntityTypeBuilder<Policy> builder)
    {
        builder.HasKey(e => e.PolicyNumber).HasName("PK__Policies__46DA0156E861CE11");

        builder.HasQueryFilter(x => x.IsDeleted == false);

        builder.HasIndex(e => e.PolicyNumber, "IX_Policies_Active").HasFilter("([IsDeleted]=(0))");

        builder.HasIndex(e => new { e.ClientId, e.BuildingId, e.BrokerId }, "UQ_Policies_Client_Building_Broker").IsUnique();

        builder.Property(e => e.PolicyNumber).HasMaxLength(256);
        builder.Property(e => e.BasePremium).HasColumnType("decimal(20, 4)");
        builder.Property(e => e.CreatedAt)
            .HasPrecision(0)
            .HasDefaultValueSql(DbConstants.SysUtcDateTime);
        builder.Property(e => e.CurrencyCode).HasMaxLength(3);
        builder.Property(e => e.EndDate).HasPrecision(0);
        builder.Property(e => e.FinalPremium).HasColumnType("decimal(20, 4)");
        builder.Property(e => e.StartDate).HasPrecision(0);
        builder.Property(e => e.Status).HasConversion<string>().HasMaxLength(64);
        builder.Property(e => e.UpdatedAt)
            .HasPrecision(0)
            .IsRowVersion()
            .HasDefaultValueSql(DbConstants.SysUtcDateTime);

        builder.HasOne(d => d.Broker).WithMany(p => p.Policies)
            .HasForeignKey(d => d.BrokerId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Policies_Brokers");

        builder.HasOne(d => d.Building).WithMany(p => p.Policies)
            .HasForeignKey(d => d.BuildingId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Policies_Building");

        builder.HasOne(d => d.Client).WithMany(p => p.Policies)
            .HasForeignKey(d => d.ClientId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Policies_Client");

        builder.HasOne(d => d.CurrencyCodeNavigation).WithMany(p => p.Policies)
            .HasForeignKey(d => d.CurrencyCode)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Policies_Currencies");
    }
}