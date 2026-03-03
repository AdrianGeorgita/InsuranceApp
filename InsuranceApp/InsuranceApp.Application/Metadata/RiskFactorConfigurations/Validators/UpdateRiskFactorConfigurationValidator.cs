using FluentValidation;
using InsuranceApp.Application.Metadata.RiskFactorConfigurations.DTOs;
using InsuranceApp.Domain.Enums;

namespace InsuranceApp.Application.Metadata.RiskFactorConfigurations.Validators;

public class UpdateRiskFactorConfigurationValidator : AbstractValidator<UpdateRiskFactorConfigurationRequest>
{
    public UpdateRiskFactorConfigurationValidator()
    {
        RuleFor(b => b.Level)
            .IsInEnum()
            .WithMessage("Level and must be a valid value.");
        RuleFor(b => b.ReferenceId)
            .NotEmpty()
            .When(b => b.Level is RiskFactorConfigurationLevel.Country
                or RiskFactorConfigurationLevel.County
                or RiskFactorConfigurationLevel.City)
            .WithMessage("ReferenceId is required for Geographic Configurations and must be a valid guid.");
        RuleFor(b => b.ReferenceId)
            .Null()
            .When(b => b.Level is not RiskFactorConfigurationLevel.Country
                and not RiskFactorConfigurationLevel.County
                and not RiskFactorConfigurationLevel.City)
            .WithMessage("ReferenceId must not be passed when it's not a Geographic Configurations.");
        RuleFor(b => b.BuildingType)
            .IsInEnum()
            .When(b => b.Level is RiskFactorConfigurationLevel.BuildingType)
            .WithMessage("BuildingType is required for BuildingType Configurations and must be a valid enum");
        RuleFor(b => b.BuildingType)
            .Null()
            .When(b => b.Level is not RiskFactorConfigurationLevel.BuildingType)
            .WithMessage("BuildingType must not passed when it's not a BuildingType Configuration");
        RuleFor(b => b.IsActive)
            .InclusiveBetween(false, true)
            .WithMessage("IsActive must be either true or false.");
    }
}