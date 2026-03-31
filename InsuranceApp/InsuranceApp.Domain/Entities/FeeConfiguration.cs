using InsuranceApp.Domain.Common.Interfaces;
using InsuranceApp.Domain.Enums;

namespace InsuranceApp.Domain.Entities;

public partial class FeeConfiguration : IAuditable
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public FeeConfigurationType Type { get; set; }

    public decimal Percentage { get; set; }

    public DateTime EffectiveFrom { get; set; }

    public DateTime? EffectiveTo { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
