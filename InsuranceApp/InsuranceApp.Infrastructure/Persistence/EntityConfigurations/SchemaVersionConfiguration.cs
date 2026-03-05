using InsuranceApp.Infrastructure.Persistence.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InsuranceApp.Infrastructure.Persistence.EntityConfigurations;

public class SchemaVersionConfiguration : IEntityTypeConfiguration<SchemaVersion>
{
    public void Configure(EntityTypeBuilder<SchemaVersion> builder)
    {
        builder.HasKey(e => e.Id).HasName("PK_SchemaVersions_Id");

        builder.Property(e => e.Applied).HasColumnType("datetime");
        builder.Property(e => e.ScriptName).HasMaxLength(255);
    }
}