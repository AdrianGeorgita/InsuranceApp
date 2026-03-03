using FluentValidation;
using InsuranceApp.Application.Metadata.Currencies.DTOs;

namespace InsuranceApp.Application.Metadata.Currencies.Validators;

public class CreateCurrencyValidator : AbstractValidator<CreateCurrencyRequest>
{
    public CreateCurrencyValidator()
    {
        RuleFor(b => b.Code)
            .NotEmpty()
            .Length(3)
            .WithMessage("Currency Code is required and must be of exactly 3 characters.");
        RuleFor(b => b.Code)
            .Matches("^(?i)[a-z]{3}");
        RuleFor(b => b.Name)
            .NotEmpty()
            .Length(1, 256)
            .WithMessage($"Name is required and must be between 1 and 256 characters.");
        RuleFor(b => b.ExchangeRateToBase)
            .NotEmpty()
            .GreaterThan(0)
            .WithMessage("ExchangeRateToBase is required and must be greater than 0.");
        RuleFor(b => b.ExchangeRateToBase)
            .PrecisionScale(10, 6, true);
        RuleFor(b => b.IsActive)
            .NotNull()
            .InclusiveBetween(false, true)
            .WithMessage("IsActive is required and must be either true or false.");
    }
}