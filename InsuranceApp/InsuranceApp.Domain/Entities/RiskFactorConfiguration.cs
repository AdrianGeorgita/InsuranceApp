using InsuranceApp.Domain.Common.Interfaces;
using InsuranceApp.Domain.Enums;

namespace InsuranceApp.Domain.Entities;

public partial class RiskFactorConfiguration : IAuditable
{
    public Guid Id { get; set; }

    public RiskFactorConfigurationLevel Level { get; set; }

    public Guid? ReferenceId { get; set; }

    public BuildingType? BuildingType { get; set; }

    public decimal AdjustmentPercentage { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
