using InsuranceApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InsuranceApp.Infrastructure.Persistence.EntityConfigurations;

public class PoliciesReplicaConfiguration : IEntityTypeConfiguration<PoliciesReplica>
{
    public void Configure(EntityTypeBuilder<PoliciesReplica> builder)
    {
        builder.HasKey(e => e.PolicyNumber).HasName("PK__Policies__46DA01560019C4E9");

        builder.ToTable("PoliciesReplica");

        builder.HasIndex(e => new { e.BrokerId, e.CurrencyCode, e.StartDate }, "IX_PoliciesReplica_Broker_Currency_StartDate");

        builder.HasIndex(e => new { e.CityName, e.CurrencyCode, e.StartDate }, "IX_PoliciesReplica_City_Currency_StartDate");

        builder.HasIndex(e => new { e.CountryName, e.CurrencyCode, e.StartDate }, "IX_PoliciesReplica_Country_Currency_StartDate");

        builder.HasIndex(e => new { e.CountyName, e.CurrencyCode, e.StartDate }, "IX_PoliciesReplica_County_Currency_StartDate");

        builder.HasIndex(e => new { e.CurrencyCode, e.StartDate }, "IX_PoliciesReplica_Currency_StartDate");

        builder.HasIndex(e => e.StartDate, "IX_PoliciesReplica_StartDate");

        builder.HasIndex(e => new { e.Status, e.StartDate }, "IX_PoliciesReplica_Status_StartDate");

        builder.Property(e => e.PolicyNumber).HasMaxLength(256);
        builder.Property(e => e.BrokerName).HasMaxLength(256);
        builder.Property(e => e.BuildingType).HasMaxLength(50);
        builder.Property(e => e.CityName).HasMaxLength(255);
        builder.Property(e => e.CountryName).HasMaxLength(255);
        builder.Property(e => e.CountyName).HasMaxLength(255);
        builder.Property(e => e.CurrencyCode).HasMaxLength(3);
        builder.Property(e => e.EndDate).HasPrecision(0);
        builder.Property(e => e.FinalPremium).HasColumnType("decimal(20, 4)");
        builder.Property(e => e.FinalPremiumInBaseCurrency).HasColumnType("decimal(20, 4)");
        builder.Property(e => e.StartDate).HasPrecision(0);
        builder.Property(e => e.Status).HasConversion<string>().HasMaxLength(64);
    }
}