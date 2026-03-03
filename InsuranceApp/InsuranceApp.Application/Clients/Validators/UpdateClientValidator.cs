using FluentValidation;
using InsuranceApp.Application.Clients.DTOs;

namespace InsuranceApp.Application.Clients.Validators;

public class UpdateClientValidator : AbstractValidator<UpdateClientRequest>
{
    public UpdateClientValidator()
    {
        RuleFor(c => c.Address)
            .Length(1, 255)
            .WithMessage("Address Length must be between 1 and 255 characters.");
        RuleFor(c => c.Name)
            .Length(1, 255)
            .WithMessage("Name length must be between 1 and 255 characters.");
        RuleFor(c => c.Phone)
            .Length(4, 15)
            .WithMessage("Phone number is required and must be between 4 and 15 characters.");
        RuleFor(b => b.Phone)
            .Matches("^\\+?[0-9]{4,14}$")
            .WithMessage("Phone must be a valid phone number.");
        RuleFor(c => c.Email)
            .EmailAddress()
            .Length(4, 255)
            .WithMessage("Email must be a valid email address.");
        RuleFor(c => c.IdentificationNumber)
            .Length(6, 20)
            .WithMessage($"IdentificationNumber must be between 6 and 20 characters.");

    }
}