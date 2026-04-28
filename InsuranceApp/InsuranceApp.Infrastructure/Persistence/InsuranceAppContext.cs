using InsuranceApp.Application.Common.Persistence;
using InsuranceApp.Domain.Entities;
using InsuranceApp.Infrastructure.Persistence.Identity;
using InsuranceApp.Infrastructure.Persistence.Internal;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InsuranceApp.Infrastructure.Persistence;

public partial class InsuranceAppContext : IdentityDbContext<User, Role, Guid>, IUnitOfWork
{
    public InsuranceAppContext()
    {
    }

    public InsuranceAppContext(DbContextOptions<InsuranceAppContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Administrator> Administrators { get; set; }

    public virtual DbSet<AuditLog> AuditLogs { get; set; }

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

    public virtual DbSet<PolicyAuditLog> PolicyAuditLogs { get; set; }

    public virtual DbSet<RiskFactorConfiguration> RiskFactorConfigurations { get; set; }

    public virtual DbSet<RiskIndicator> RiskIndicators { get; set; }

    public virtual DbSet<SchemaVersion> SchemaVersions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(InsuranceAppContext).Assembly);
    }
}
