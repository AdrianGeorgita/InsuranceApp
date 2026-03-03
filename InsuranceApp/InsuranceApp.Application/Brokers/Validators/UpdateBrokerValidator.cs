using FluentValidation;
using InsuranceApp.Application.Brokers.DTOs;

namespace InsuranceApp.Application.Brokers.Validators;

public class UpdateBrokerValidator : AbstractValidator<UpdateBrokerRequest>
{
    public UpdateBrokerValidator()
    {
        RuleFor(b => b.Code)
            .Length(8, 128)
            .WithMessage("Broker Code must be between 8 and 128 characters.");
        RuleFor(b => b.Code)
            .Matches("^(?i)brk-[A-Z]{2}-\\d+$")
            .WithMessage("Code is not in the correct format: BRK-AZ-000.");
        RuleFor(b => b.Name)
            .Length(1, 256)
            .WithMessage($"Name must be between 1 and 256 characters.");
        RuleFor(b => b.Phone)
            .Length(4, 15)
            .WithMessage("Phone number must be between 4 and 15 characters.");
        RuleFor(b => b.Phone)
            .Matches("^\\+?[0-9]{4,14}$")
            .WithMessage("Phone must be a valid phone number.");
        RuleFor(b => b.Email)
            .EmailAddress()
            .Length(4, 255)
            .WithMessage("Email must be a valid email address.");
        RuleFor(b => b.CommissionPercentage)
            .GreaterThanOrEqualTo(0.0000M)
            .WithMessage("CommissionPercentage must be greater than or equal to 0.0");
        RuleFor(b => b.CommissionPercentage)
            .PrecisionScale(10, 6, true);
    }
}