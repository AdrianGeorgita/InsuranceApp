using FluentValidation;
using InsuranceApp.Application.Auth.DTOs;

namespace InsuranceApp.Application.Auth.Validators;

public class LoginValidator : AbstractValidator<LoginRequest>
{
    public LoginValidator()
    {
        RuleFor(b => b.Email)
            .NotEmpty()
            .EmailAddress()
            .Length(4, 255)
            .WithMessage("Email is required and must be a valid email address.");
        RuleFor(b => b.Password)
            .NotEmpty()
            .WithMessage("Password is required!");
    }
}