using InsuranceApp.Application.Common.Persistence;
using InsuranceApp.Domain.Entities;
using InsuranceApp.Domain.Enums;
using InsuranceApp.Infrastructure.Persistence.Internal;
using Microsoft.EntityFrameworkCore;

namespace InsuranceApp.Infrastructure.Persistence;

public partial class InsuranceAppContext : DbContext, IUnitOfWork
{
    public InsuranceAppContext()
    {
    }

    public InsuranceAppContext(DbContextOptions<InsuranceAppContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Administrator> Administrators { get; set; }

    public virtual DbSet<AuditTableValueChange> AuditTableValueChanges { get; set; }

    public virtual DbSet<Broker> Brokers { get; set; }

    public virtual DbSet<Building> Buildings { get; set; }

    public virtual DbSet<City> Cities { get; set; }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<Country> Countries { get; set; }

    public virtual DbSet<County> Counties { get; set; }

    public virtual DbSet<Currency> Currencies { get; set; }

    public virtual DbSet<FeeConfiguration> FeeConfigurations { get; set; }

    public virtual DbSet<Policy> Policies { get; set; }

    public virtual DbSet<PoliciesReplica> PoliciesReplicas { get; set; }

    public virtual DbSet<RiskFactorConfiguration> RiskFactorConfigurations { get; set; }

    public virtual DbSet<RiskIndicator> RiskIndicators { get; set; }

    public virtual DbSet<SchemaVersion> SchemaVersions { get; set; }

    private const string SysUtcDateTime = "(sysutcdatetime())";

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Administrator>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Administ__3214EC079F10394C");

            entity.HasIndex(e => e.Email, "UQ_Administrators_Email").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql(SysUtcDateTime);
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.Name).HasMaxLength(256);
            entity.Property(e => e.Role).HasMaxLength(64);
            entity.Property(e => e.UpdatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql(SysUtcDateTime);
        });

        modelBuilder.Entity<AuditTableValueChange>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AuditTab__3214EC0751523691");

            entity.HasIndex(e => new { e.EventId, e.UpdatedAt }, "IX_AuditTableValueChanges_EventId").IsDescending(false, true);

            entity.HasIndex(e => new { e.TableName, e.UpdatedAt }, "IX_AuditTableValueChanges_TableName").IsDescending(false, true);

            entity.HasIndex(e => new { e.UserId, e.UpdatedAt }, "IX_AuditTableValueChanges_User").IsDescending(false, true);

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.ColumnName).HasMaxLength(128);
            entity.Property(e => e.RowId).HasMaxLength(256);
            entity.Property(e => e.Reason).HasMaxLength(512);
            entity.Property(e => e.TableName).HasMaxLength(128);
            entity.Property(e => e.UpdatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql(SysUtcDateTime);
        });

        modelBuilder.Entity<Broker>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Brokers__3214EC07BEE2B83E");

            entity.HasIndex(e => e.Code, "UQ_Brokers_Code").IsUnique();

            entity.HasIndex(e => e.Email, "UQ_Brokers_Email").IsUnique();

            entity.HasIndex(e => e.Phone, "UQ_Brokers_Phone").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Code).HasMaxLength(128);
            entity.Property(e => e.CommissionPercentage)
                .HasDefaultValue(0.0m)
                .HasColumnType("decimal(10, 6)");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql(SysUtcDateTime);
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.Name).HasMaxLength(256);
            entity.Property(e => e.Phone).HasMaxLength(15);
            entity.Property(e => e.UpdatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql(SysUtcDateTime);
            entity.Property(e => e.Status)
                .HasConversion(
                    v => v == BrokerStatus.Active,
                    v => v ? BrokerStatus.Active : BrokerStatus.Inactive
                );
        });

        modelBuilder.Entity<Building>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Building__3214EC075A47808A");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.BuildingType).HasMaxLength(50);
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql(SysUtcDateTime);
            entity.Property(e => e.InsuredValue).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.SurfaceArea).HasColumnType("decimal(6, 2)");
            entity.Property(e => e.UpdatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql(SysUtcDateTime);

            entity.HasOne(d => d.City).WithMany(p => p.Buildings)
                .HasForeignKey(d => d.CityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Buildings_City");

            entity.HasOne(d => d.Owner).WithMany(p => p.Buildings)
                .HasForeignKey(d => d.OwnerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Buildings_Owner");

            entity.HasMany(d => d.RiskIndicators).WithMany(p => p.Buildings)
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
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Cities__3214EC0788A2A5E6");

            entity.HasIndex(e => new { e.CountyId, e.Name }, "UQ_Cities_CountyId_Name").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql(SysUtcDateTime);
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql(SysUtcDateTime);

            entity.HasOne(d => d.County).WithMany(p => p.Cities)
                .HasForeignKey(d => d.CountyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Cities_Counties");
        });

        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Clients__3214EC078E09FC27");

            entity.HasIndex(e => e.IdentificationNumber, "UQ_Clients_IdentificationNumber").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql(SysUtcDateTime);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.IdentificationNumber).HasMaxLength(20);
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(15);
            entity.Property(e => e.Type).HasMaxLength(20);
            entity.Property(e => e.UpdatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql(SysUtcDateTime);
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Countrie__3214EC07AAC5CB0A");

            entity.HasIndex(e => e.Iso2, "UQ_Countries_ISO2").IsUnique();

            entity.HasIndex(e => e.Iso3, "UQ_Countries_ISO3").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql(SysUtcDateTime);
            entity.Property(e => e.Iso2)
                .HasMaxLength(2)
                .HasColumnName("ISO2");
            entity.Property(e => e.Iso3)
                .HasMaxLength(3)
                .HasColumnName("ISO3");
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql(SysUtcDateTime);
        });

        modelBuilder.Entity<County>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Counties__3214EC074FEF11ED");

            entity.HasIndex(e => new { e.CountryId, e.Name }, "UQ_Counties_CountryId_Name").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql(SysUtcDateTime);
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql(SysUtcDateTime);

            entity.HasOne(d => d.Country).WithMany(p => p.Counties)
                .HasForeignKey(d => d.CountryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Counties_Countries");
        });

        modelBuilder.Entity<Currency>(entity =>
        {
            entity.HasKey(e => e.Code).HasName("PK__Currenci__A25C5AA6E4DC565C");

            entity.Property(e => e.Code).HasMaxLength(3);
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql(SysUtcDateTime);
            entity.Property(e => e.ExchangeRateToBase).HasColumnType("decimal(10, 6)");
            entity.Property(e => e.Name).HasMaxLength(256);
            entity.Property(e => e.UpdatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql(SysUtcDateTime);
        });

        modelBuilder.Entity<FeeConfiguration>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__FeeConfi__3214EC073A438B8E");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql(SysUtcDateTime);
            entity.Property(e => e.EffectiveFrom).HasPrecision(0);
            entity.Property(e => e.EffectiveTo).HasPrecision(0);
            entity.Property(e => e.Name).HasMaxLength(256);
            entity.Property(e => e.Percentage).HasColumnType("decimal(10, 6)");
            entity.Property(e => e.Type).HasConversion<string>().HasMaxLength(128);
            entity.Property(e => e.UpdatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql(SysUtcDateTime);
        });

        modelBuilder.Entity<Policy>(entity =>
        {
            entity.HasKey(e => e.PolicyNumber).HasName("PK__Policies__46DA0156E861CE11");

            entity.HasIndex(e => new { e.ClientId, e.BuildingId, e.BrokerId }, "UQ_Policies_Client_Building_Broker").IsUnique();

            entity.Property(e => e.PolicyNumber).HasMaxLength(256);
            entity.Property(e => e.BasePremium).HasColumnType("decimal(20, 4)");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql(SysUtcDateTime);
            entity.Property(e => e.CurrencyCode).HasMaxLength(3);
            entity.Property(e => e.EndDate).HasPrecision(0);
            entity.Property(e => e.FinalPremium).HasColumnType("decimal(20, 4)");
            entity.Property(e => e.StartDate).HasPrecision(0);
            entity.Property(e => e.Status).HasConversion<string>().HasMaxLength(64);
            entity.Property(e => e.UpdatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql(SysUtcDateTime);

            entity.HasOne(d => d.Broker).WithMany(p => p.Policies)
                .HasForeignKey(d => d.BrokerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Policies_Brokers");

            entity.HasOne(d => d.Building).WithMany(p => p.Policies)
                .HasForeignKey(d => d.BuildingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Policies_Building");

            entity.HasOne(d => d.Client).WithMany(p => p.Policies)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Policies_Client");

            entity.HasOne(d => d.CurrencyCodeNavigation).WithMany(p => p.Policies)
                .HasForeignKey(d => d.CurrencyCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Policies_Currencies");
        });

        modelBuilder.Entity<PoliciesReplica>(entity =>
        {
            entity.HasKey(e => e.PolicyNumber).HasName("PK__Policies__46DA01560019C4E9");

            entity.ToTable("PoliciesReplica");

            entity.HasIndex(e => new { e.BrokerId, e.CurrencyCode, e.StartDate }, "IX_PoliciesReplica_Broker_Currency_StartDate");

            entity.HasIndex(e => new { e.CityName, e.CurrencyCode, e.StartDate }, "IX_PoliciesReplica_City_Currency_StartDate");

            entity.HasIndex(e => new { e.CountryName, e.CurrencyCode, e.StartDate }, "IX_PoliciesReplica_Country_Currency_StartDate");

            entity.HasIndex(e => new { e.CountyName, e.CurrencyCode, e.StartDate }, "IX_PoliciesReplica_County_Currency_StartDate");

            entity.HasIndex(e => new { e.CurrencyCode, e.StartDate }, "IX_PoliciesReplica_Currency_StartDate");

            entity.HasIndex(e => e.StartDate, "IX_PoliciesReplica_StartDate");

            entity.HasIndex(e => new { e.Status, e.StartDate }, "IX_PoliciesReplica_Status_StartDate");

            entity.Property(e => e.PolicyNumber).HasMaxLength(256);
            entity.Property(e => e.BrokerName).HasMaxLength(256);
            entity.Property(e => e.BuildingType).HasMaxLength(50);
            entity.Property(e => e.CityName).HasMaxLength(255);
            entity.Property(e => e.CountryName).HasMaxLength(255);
            entity.Property(e => e.CountyName).HasMaxLength(255);
            entity.Property(e => e.CurrencyCode).HasMaxLength(3);
            entity.Property(e => e.EndDate).HasPrecision(0);
            entity.Property(e => e.FinalPremium).HasColumnType("decimal(20, 4)");
            entity.Property(e => e.FinalPremiumInBaseCurrency).HasColumnType("decimal(20, 4)");
            entity.Property(e => e.StartDate).HasPrecision(0);
            entity.Property(e => e.Status).HasConversion<string>().HasMaxLength(64);
        });

        modelBuilder.Entity<RiskFactorConfiguration>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RiskFact__3214EC070003BBFA");

            entity.HasIndex(e => new { e.Level, e.ReferenceId, e.BuildingType }, "UQ_RiskFactorConfigurations_Level_ReferenceId_BuildingType").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.AdjustmentPercentage).HasColumnType("decimal(10, 6)");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql(SysUtcDateTime);
            entity.Property(e => e.Level).HasConversion<string>().HasMaxLength(64);
            entity.Property(e => e.BuildingType).HasConversion<string>().HasMaxLength(50);
            entity.Property(e => e.UpdatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql(SysUtcDateTime);
        });

        modelBuilder.Entity<RiskIndicator>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RiskIndi__3214EC0705F8A6BF");

            entity.HasIndex(e => e.Name, "UQ_RiskIndicators_Name").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql(SysUtcDateTime);
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql(SysUtcDateTime);
        });

        modelBuilder.Entity<SchemaVersion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_SchemaVersions_Id");

            entity.Property(e => e.Applied).HasColumnType("datetime");
            entity.Property(e => e.ScriptName).HasMaxLength(255);
        });
    }
}
