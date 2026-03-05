using InsuranceApp.Domain.Entities;
using InsuranceApp.Infrastructure.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InsuranceApp.Infrastructure.Persistence.EntityConfigurations;

public class AdministratorConfiguration : IEntityTypeConfiguration<Administrator>
{
    public void Configure(EntityTypeBuilder<Administrator> builder)
    {
        builder.HasKey(e => e.Id).HasName("PK__Administ__3214EC079F10394C");

        builder.HasIndex(e => e.Email, "UQ_Administrators_Email").IsUnique();

        builder.Property(e => e.Id).ValueGeneratedNever();
        builder.Property(e => e.CreatedAt)
            .HasPrecision(0)
            .HasDefaultValueSql(DbConstants.SysUtcDateTime);
        builder.Property(e => e.Email).HasMaxLength(256);
        builder.Property(e => e.Name).HasMaxLength(256);
        builder.Property(e => e.Role).HasMaxLength(64);
        builder.Property(e => e.UpdatedAt)
            .HasPrecision(0)
            .IsRowVersion()
            .HasDefaultValueSql(DbConstants.SysUtcDateTime);
    }
}