using FluentValidation;
using FluentValidation.TestHelper;
using InsuranceApp.Application.Clients.DTOs;
using InsuranceApp.Application.Clients.Validators;
using InsuranceApp.Domain.Enums;

namespace InsuranceApp.UnitTests.Validators;

public class CreateClientValidatorTests
{

    private readonly IValidator<CreateClientRequest> _validator = new CreateClientValidator();

    [Fact]
    public async Task ValidateAsync_ValidRequest_ShouldNotHaveError()
    {
        var req = ValidRequest();

        var result = await _validator.TestValidateAsync(req);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("1234")]
    [InlineData("123123123123213213213213213132123")]
    [InlineData("")]
    public async Task ValidateAsync_InvalidIdentificationNumber_ShouldHaveError(string identificationNumber)
    {
        var req = ValidRequest();
        req.IdentificationNumber = identificationNumber;

        var result = await _validator.TestValidateAsync(req);

        result.ShouldHaveValidationErrorFor(b => b.IdentificationNumber)
            .WithErrorMessage("IdentificationNumber must be between 6 and 20 characters.");
    }

    [Theory]
    [InlineData("134")]
    [InlineData("123123123123213213213213213132123")]
    [InlineData("")]
    public async Task ValidateAsync_InvalidPhone_ShouldHaveError(string phone)
    {
        var req = ValidRequest();
        req.Phone = phone;

        var result = await _validator.TestValidateAsync(req);

        result.ShouldHaveValidationErrorFor(b => b.Phone)
            .WithErrorMessage("Phone number is required and must be between 4 and 15 characters.");
    }


    [Fact]
    public async Task ValidateAsync_InvalidAddress_ShouldHaveError()
    {
        const string invalidAddress = "veryLongAddressveryLongAddressveryLongAddressveryLongAddressveryLongAddress" +
                                      "veryLongAddressveryLongAddressveryLongAddressveryLongAddressveryLongAddress" +
                                      "veryLongAddressveryLongAddressveryLongAddressveryLongAddressveryLongAddressveryLongAddress" +
                                      "veryLongAddressveryLongAddressveryLongAddressveryLongAddressveryLongAddress";
        var req = ValidRequest();
        req.Address = invalidAddress;

        var result = await _validator.TestValidateAsync(req);

        result.ShouldHaveValidationErrorFor(b => b.Address)
            .WithErrorMessage("Address Length must be between 0 and 255 characters.");
    }
    private CreateClientRequest ValidRequest() => new CreateClientRequest
    {
        Name = "John Doe",
        Address = "Some street Nr.7",
        Email = "john.doe@mail.com",
        IdentificationNumber = "3020202123123",
        Phone = "0712345678",
        Type = ClientType.Individual
    };
}
