using InsuranceApp.Infrastructure.Persistence.Constants;
using InsuranceApp.Infrastructure.Persistence.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InsuranceApp.Infrastructure.Persistence.EntityConfigurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(e => e.Id).HasName("PK__Users__3214EC074D4AD8BC");

        builder.HasIndex(e => e.NormalizedEmail, "IX_Users_NormalizedEmail").IsUnique();

        builder.Property(e => e.Id).ValueGeneratedNever();
        builder.Property(e => e.ConcurrencyStamp).HasMaxLength(36);
        builder.Property(e => e.CreatedAt)
            .HasPrecision(0)
            .HasDefaultValueSql(DbConstants.SysUtcDateTime);
        builder.Property(e => e.DeletedAt).HasPrecision(0);
        builder.Property(e => e.Email).HasMaxLength(256);
        builder.Property(e => e.NormalizedEmail).HasMaxLength(256);
        builder.Property(e => e.NormalizedUserName).HasMaxLength(256);
        builder.Property(e => e.SecurityStamp).HasMaxLength(36);
        builder.Property(e => e.UpdatedAt)
            .HasPrecision(0)
            .HasDefaultValueSql(DbConstants.SysUtcDateTime);
        builder.Property(e => e.UserName).HasMaxLength(256);
    }
}