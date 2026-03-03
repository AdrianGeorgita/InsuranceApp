using InsuranceApp.Domain.Enums;

namespace InsuranceApp.Application.Metadata.FeeConfigurations.DTOs;

public class UpdateFeeConfigurationRequest
{
    public string? Name { get; set; }

    public FeeConfigurationType? Type { get; set; }

    public decimal? Percentage { get; set; }

    public DateTime? EffectiveFrom { get; set; }

    public DateTime? EffectiveTo { get; set; }

    public bool? IsActive { get; set; }
}