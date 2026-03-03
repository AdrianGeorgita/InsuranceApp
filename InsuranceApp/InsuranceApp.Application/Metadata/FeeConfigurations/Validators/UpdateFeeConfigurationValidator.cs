using FluentValidation;
using InsuranceApp.Application.Common.Repository;
using InsuranceApp.Application.Metadata.FeeConfigurations.DTOs;
using InsuranceApp.Domain.Enums;

namespace InsuranceApp.Application.Metadata.FeeConfigurations.Validators;

public class UpdateFeeConfigurationValidator : AbstractValidator<UpdateFeeConfigurationRequest>
{
    public UpdateFeeConfigurationValidator(IRiskIndicatorRepository riskIndicatorRepository)
    {
        RuleFor(b => b.Name)
            .Length(1, 256)
            .WithMessage("Name must be between 1 and 256 characters.");
        RuleFor(b => b.Type)
            .IsInEnum()
            .WithMessage($"Type must be a valid value.");
        RuleFor(b => b.Name)
            .MustAsync(async (name, ct) =>
            {
                return await riskIndicatorRepository.ExistsAsync(r => (r.Name + " adjustment").ToLower() == name!.ToLower(), ct);
            })
            .When(b => b.Type == FeeConfigurationType.RiskAdjustment)
            .When(b => b.Name != null)
            .WithMessage($"Risk Indicator does not exist.");
        RuleFor(b => b.Name)
            .Must(name => name!.EndsWith("adjustment"))
            .When(b => b.Type == FeeConfigurationType.RiskAdjustment)
            .When(b => b.Name != null)
            .WithMessage("Name must end with 'adjustment' when adding a RiskAdjustment.");
        RuleFor(b => b.Percentage)
            .GreaterThan(0)
            .WithMessage("Percentage must be greater than 0.");
        RuleFor(b => b.IsActive)
            .InclusiveBetween(false, true)
            .WithMessage("IsActive must be either true or false.");
    }
}