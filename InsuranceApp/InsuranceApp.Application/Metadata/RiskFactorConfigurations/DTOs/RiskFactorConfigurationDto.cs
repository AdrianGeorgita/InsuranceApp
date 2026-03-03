using InsuranceApp.Domain.Enums;

namespace InsuranceApp.Application.Metadata.RiskFactorConfigurations.DTOs;

public class RiskFactorConfigurationDto
{
    public Guid Id { get; set; }

    public RiskFactorConfigurationLevel Level { get; set; }

    public Guid? ReferenceId { get; set; }
    public BuildingType? BuildingType { get; set; }

    public decimal AdjustmentPercentage { get; set; }

    public bool IsActive { get; set; }
}