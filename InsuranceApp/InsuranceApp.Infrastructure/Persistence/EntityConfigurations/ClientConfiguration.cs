using InsuranceApp.Domain.Entities;
using InsuranceApp.Infrastructure.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InsuranceApp.Infrastructure.Persistence.EntityConfigurations;

public class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.HasKey(e => e.Id).HasName("PK__Clients__3214EC078E09FC27");

        builder.HasQueryFilter(x => x.IsDeleted == false);

        builder.HasIndex(e => e.Id, "IX_Clients_Active").HasFilter("([IsDeleted]=(0))");

        builder.HasIndex(e => e.IdentificationNumber, "UQ_Clients_IdentificationNumber").IsUnique();

        builder.Property(e => e.Id).ValueGeneratedNever();
        builder.Property(e => e.Address).HasMaxLength(255);
        builder.Property(e => e.CreatedAt)
            .HasPrecision(0)
            .HasDefaultValueSql(DbConstants.SysUtcDateTime);
        builder.Property(e => e.Email).HasMaxLength(255);
        builder.Property(e => e.IdentificationNumber).HasMaxLength(20);
        builder.Property(e => e.Name).HasMaxLength(255);
        builder.Property(e => e.Phone).HasMaxLength(15);
        builder.Property(e => e.Type).HasMaxLength(20);
        builder.Property(e => e.UpdatedAt)
            .HasPrecision(0)
            .IsRowVersion()
            .HasDefaultValueSql(DbConstants.SysUtcDateTime);
    }
}