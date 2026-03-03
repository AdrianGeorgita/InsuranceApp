using FluentValidation;
using InsuranceApp.Application.Common.Repository;
using InsuranceApp.Application.Metadata.FeeConfigurations.DTOs;
using InsuranceApp.Domain.Enums;

namespace InsuranceApp.Application.Metadata.FeeConfigurations.Validators;

public class CreateFeeConfigurationValidator : AbstractValidator<CreateFeeConfigurationRequest>
{
    public CreateFeeConfigurationValidator(IRiskIndicatorRepository riskIndicatorRepository)
    {
        RuleFor(b => b.Name)
            .NotEmpty()
            .Length(1, 256)
            .WithMessage("Name is required and must be between 1 and 256 characters.");
        RuleFor(b => b.Type)
            .NotNull()
            .IsInEnum()
            .WithMessage($"Type is required and must be a valid value.");
        RuleFor(b => b.Name)
            .MustAsync(async (name, ct) =>
            {
                return await riskIndicatorRepository.ExistsAsync(r => (r.Name + " adjustment").ToLower() == name.ToLower(), ct);
            })
            .When(b => b.Type == FeeConfigurationType.RiskAdjustment)
            .WithMessage($"Risk Indicator does not exist.");
        RuleFor(b => b.Name)
            .Must(name => name.EndsWith("adjustment"))
            .When(b => b.Type == FeeConfigurationType.RiskAdjustment)
            .WithMessage("Name must end with 'adjustment' when adding a RiskAdjustment.");
        RuleFor(b => b.Percentage)
            .NotEmpty()
            .GreaterThan(0)
            .WithMessage("Percentage is required and must be greater than 0.");
        RuleFor(b => b.EffectiveFrom)
            .NotEmpty()
            .WithMessage("EffectiveFrom is required and must be a valid date.");
        RuleFor(b => b.EffectiveTo)
            .GreaterThan(b => b.EffectiveFrom)
            .WithMessage("EffectiveTo must be a date after the EffectiveFrom date.");
        RuleFor(b => b.IsActive)
            .NotNull()
            .InclusiveBetween(false, true)
            .WithMessage("IsActive is required and must be either true or false.");
    }
}