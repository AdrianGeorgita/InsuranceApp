using FluentValidation;
using InsuranceApp.Application.Common.Repository;
using InsuranceApp.Application.Reports.DTOs;

namespace InsuranceApp.Application.Reports.Validators;

public class ReportFilterValidator : AbstractValidator<ReportFilter>
{
    public ReportFilterValidator(ICurrencyRepository currencyRepository)
    {
        RuleFor(b => b.From)
            .NotEmpty()
            .WithMessage("From Date is required.")
            .LessThan(DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("From Date must be in the past.");
        RuleFor(b => b.To)
            .NotEmpty()
            .WithMessage("To Date is required.");
        RuleFor(b => b.To)
            .GreaterThanOrEqualTo(b => b.From)
            .WithMessage("From Date must be before the To Date.");
        RuleFor(b => b.Currency)
            .Length(3)
            .WithMessage("Currency must have a length of 3 characters");
        RuleFor(b => b.Currency)
            .Matches("^(?i)[a-z]{3}");
        RuleFor(b => b.Currency)
            .MustAsync(async (currencyCode, ct) =>
            {
                return await currencyRepository.ExistsAsync(c => c.Code == currencyCode
                    && c.IsActive, ct);
            })
            .When(b => b.Currency is not null)
            .WithMessage($"Currency does not exist.");
        RuleFor(b => b.Status)
            .IsInEnum()
            .WithMessage($"Status must be a valid value.");
        RuleFor(b => b.BuildingType)
            .IsInEnum()
            .WithMessage($"BuildingType must be a valid value.");
    }
}