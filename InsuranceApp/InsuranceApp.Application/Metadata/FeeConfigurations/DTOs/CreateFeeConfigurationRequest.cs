using InsuranceApp.Domain.Enums;

namespace InsuranceApp.Application.Metadata.FeeConfigurations.DTOs;

public class CreateFeeConfigurationRequest
{
    public string Name { get; set; } = null!;

    public FeeConfigurationType Type { get; set; }

    public decimal Percentage { get; set; }

    public DateTime EffectiveFrom { get; set; }

    public DateTime? EffectiveTo { get; set; }

    public bool IsActive { get; set; }
}