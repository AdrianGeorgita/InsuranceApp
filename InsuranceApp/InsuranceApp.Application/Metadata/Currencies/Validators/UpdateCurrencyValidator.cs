using FluentValidation;
using InsuranceApp.Application.Metadata.Currencies.DTOs;

namespace InsuranceApp.Application.Metadata.Currencies.Validators;

public class UpdateCurrencyValidator : AbstractValidator<UpdateCurrencyRequest>
{
    public UpdateCurrencyValidator()
    {
        RuleFor(b => b.Code)
            .Length(3)
            .WithMessage("Currency Code must be of exactly 3 characters.");
        RuleFor(b => b.Code)
            .Matches("^(?i)[a-z]{3}");
        RuleFor(b => b.Name)
            .Length(1, 256)
            .WithMessage($"Name must be between 1 and 256 characters.");
        RuleFor(b => b.ExchangeRateToBase)
            .GreaterThan(0)
            .WithMessage("ExchangeRateToBase must be greater than 0.");
        RuleFor(b => b.ExchangeRateToBase)
            .PrecisionScale(10, 6, true);
        RuleFor(b => b.IsActive)
            .InclusiveBetween(false, true)
            .WithMessage("IsActive and must be either true or false.");
    }
}