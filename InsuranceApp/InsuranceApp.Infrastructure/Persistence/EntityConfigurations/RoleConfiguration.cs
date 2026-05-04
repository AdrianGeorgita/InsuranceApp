using InsuranceApp.Infrastructure.Persistence.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InsuranceApp.Infrastructure.Persistence.EntityConfigurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");
        builder.HasKey(e => e.Id).HasName("PK__Roles__3214EC075F8F6B5F");

        builder.HasIndex(e => e.NormalizedName, "IX_Roles_NormalizedName").IsUnique();

        builder.Property(e => e.Id).ValueGeneratedNever();
        builder.Property(e => e.ConcurrencyStamp).HasMaxLength(36);
        builder.Property(e => e.Name).HasMaxLength(256);
        builder.Property(e => e.NormalizedName).HasMaxLength(256);
    }
}