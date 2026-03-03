using FluentValidation;
using InsuranceApp.Application.Policies.DTOs;
using InsuranceApp.Domain.Enums;

namespace InsuranceApp.Application.Policies.Validators;

public class UpdatePolicyValidator : AbstractValidator<UpdatePolicyRequest>
{
    public UpdatePolicyValidator()
    {
        RuleFor(b => b.Reason)
            .NotEmpty()
            .Length(1, 512)
            .When(b => b.NewStatus == PolicyStatus.Cancelled)
            .WithMessage("Reason is required when cancelling a Policy and must be between 1 and 512 characters.");
        RuleFor(b => b.NewStatus)
            .NotNull()
            .IsInEnum()
            .WithMessage("NewStatus is required and must be a valid enum value.");
    }
}