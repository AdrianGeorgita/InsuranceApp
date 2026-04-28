using FluentValidation;
using InsuranceApp.Application.Auth.DTOs;

namespace InsuranceApp.Application.Auth.Validators;

public class RegisterValidator : AbstractValidator<RegisterRequest>
{
    public RegisterValidator()
    {
        RuleFor(b => b.Username)
            .NotEmpty()
            .Length(8, 256)
            .WithMessage("Username is required and must be between 8 and 256 characters.");
        RuleFor(b => b.Email)
            .NotEmpty()
            .EmailAddress()
            .Length(4, 255)
            .WithMessage("Email is required and must be a valid email address.");
        RuleFor(b => b.Role)
            .NotEmpty()
            .WithMessage("Role is required!");
        RuleFor(b => b.Password)
            .NotEmpty()
            .WithMessage("Password is required!");
    }
}