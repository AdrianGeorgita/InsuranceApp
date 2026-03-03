using FluentValidation;
using InsuranceApp.Application.Clients.DTOs;

namespace InsuranceApp.Application.Clients.Validators;

public class CreateClientValidator : AbstractValidator<CreateClientRequest>
{
    public CreateClientValidator()
    {
        RuleFor(c => c.Address)
            .Length(0, 255)
            .WithMessage("Address Length must be between 0 and 255 characters.");
        RuleFor(c => c.IdentificationNumber)
            .NotEmpty()
            .Length(6, 20)
            .WithMessage($"IdentificationNumber must be between 6 and 20 characters.");
        RuleFor(c => c.Phone)
            .NotEmpty()
            .Length(4, 15)
            .WithMessage("Phone number is required and must be between 4 and 15 characters.");
        RuleFor(b => b.Phone)
            .Matches("^\\+?[0-9]{4,14}$")
            .WithMessage("Phone must be a valid phone number.");
        RuleFor(c => c.Email)
            .NotEmpty()
            .EmailAddress()
            .Length(4, 255)
            .WithMessage("Email must be a valid email address.");
        RuleFor(c => c.Type)
            .NotNull()
            .IsInEnum()
            .WithMessage("Type must be a valid ClientType.");
    }
}