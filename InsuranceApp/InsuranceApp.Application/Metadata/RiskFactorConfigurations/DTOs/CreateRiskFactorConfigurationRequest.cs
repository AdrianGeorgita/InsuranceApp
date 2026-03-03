using InsuranceApp.Domain.Enums;

namespace InsuranceApp.Application.Metadata.RiskFactorConfigurations.DTOs;

public class CreateRiskFactorConfigurationRequest
{
    public RiskFactorConfigurationLevel Level { get; set; }

    public Guid? ReferenceId { get; set; }

    public BuildingType? BuildingType { get; set; }

    public decimal AdjustmentPercentage { get; set; }

    public bool IsActive { get; set; }
}