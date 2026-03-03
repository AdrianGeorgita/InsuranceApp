using FluentValidation;
using FluentValidation.TestHelper;
using InsuranceApp.Application.Brokers.DTOs;
using InsuranceApp.Application.Brokers.Validators;

namespace InsuranceApp.UnitTests.Validators;

public class UpdateBrokerValidatorTests
{

    private readonly IValidator<UpdateBrokerRequest> _validator = new UpdateBrokerValidator();

    [Fact]
    public async Task ValidateAsync_ValidRequest_ShouldNotHaveError()
    {
        var req = ValidRequest();

        var result = await _validator.TestValidateAsync(req);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("SomeCodeHereSomeCodeHereSomeCodeHereSomeCodeHereSomeCodeHere" +
                "SomeCodeHereSomeCodeHereSomeCodeHereSomeCodeHereSomeCodeHere" +
                "SomeCodeHereSomeCodeHereSomeCodeHereSomeCodeHereSomeCodeHere")]
    [InlineData("")]
    public async Task ValidateAsync_InvalidCodeLength_ShouldHaveError(string code)
    {
        var req = ValidRequest();
        req.Code = code;

        var result = await _validator.TestValidateAsync(req);

        result.ShouldHaveValidationErrorFor(b => b.Code)
            .WithErrorMessage("Broker Code must be between 8 and 128 characters.");
    }

    [Theory]
    [InlineData("BR-RO-001")]
    [InlineData("BRKRO001")]
    [InlineData("SomeCodeHere")]
    public async Task ValidateAsync_InvalidCodeFormat_ShouldHaveError(string code)
    {
        var req = ValidRequest();
        req.Code = code;

        var result = await _validator.TestValidateAsync(req);

        result.ShouldHaveValidationErrorFor(b => b.Code)
            .WithErrorMessage("Code is not in the correct format: BRK-AZ-000.");
    }

    [Theory]
    [InlineData("134")]
    [InlineData("123123123123213213213213213132123")]
    [InlineData("")]
    public async Task ValidateAsync_InvalidPhoneLength_ShouldHaveError(string phone)
    {
        var req = ValidRequest();
        req.Phone = phone;

        var result = await _validator.TestValidateAsync(req);

        result.ShouldHaveValidationErrorFor(b => b.Phone)
            .WithErrorMessage("Phone number must be between 4 and 15 characters.");
    }

    [Theory]
    [InlineData("++phone123")]
    [InlineData("-0712342")]
    public async Task ValidateAsync_InvalidPhoneFormat_ShouldHaveError(string phone)
    {
        var req = ValidRequest();
        req.Phone = phone;

        var result = await _validator.TestValidateAsync(req);

        result.ShouldHaveValidationErrorFor(b => b.Phone)
            .WithErrorMessage("Phone must be a valid phone number.");
    }


    [Theory]
    [InlineData("")]
    [InlineData("SomeRandomNameSomeRandomNameSomeRandomNameSomeRandomNameSomeRandomName" +
                "SomeRandomNameSomeRandomNameSomeRandomNameSomeRandomNameSomeRandomNameSomeRandomName" +
                "SomeRandomNameSomeRandomNameSomeRandomNameSomeRandomNameSomeRandomNameSomeRandomName" +
                "SomeRandomNameSomeRandomNameSomeRandomNameSomeRandomNameSomeRandomNameSomeRandomName")]
    public async Task ValidateAsync_InvalidName_ShouldHaveError(string name)
    {
        var req = ValidRequest();
        req.Name = name;

        var result = await _validator.TestValidateAsync(req);

        result.ShouldHaveValidationErrorFor(b => b.Name)
            .WithErrorMessage("Name must be between 1 and 256 characters.");
    }

    [Theory]
    [InlineData("")]
    [InlineData("a@b")]
    public async Task ValidateAsync_InvalidEmail_ShouldHaveError(string email)
    {
        var req = ValidRequest();
        req.Email = email;

        var result = await _validator.TestValidateAsync(req);

        result.ShouldHaveValidationErrorFor(b => b.Email)
            .WithErrorMessage("Email must be a valid email address.");
    }

    [Fact]
    public async Task ValidateAsync_InvalidEmailFormat_ShouldHaveError()
    {
        const string invalidEmail = "johndoe.example.com";
        var req = ValidRequest();
        req.Email = invalidEmail;

        var result = await _validator.TestValidateAsync(req);

        result.ShouldHaveValidationErrorFor(b => b.Email)
            .WithErrorMessage("'Email' is not a valid email address.");
    }

    [Fact]
    public async Task ValidateAsync_InvalidCommissionPercentageValue_ShouldHaveError()
    {
        var req = ValidRequest();
        req.CommissionPercentage = -1.0M;

        var result = await _validator.TestValidateAsync(req);

        result.ShouldHaveValidationErrorFor(b => b.CommissionPercentage)
            .WithErrorMessage("CommissionPercentage must be greater than or equal to 0.0");
    }

    [Fact]
    public async Task ValidateAsync_InvalidCommissionPercentagePrecision_ShouldHaveError()
    {
        var req = ValidRequest();
        req.CommissionPercentage = 0.32131233M;

        var result = await _validator.TestValidateAsync(req);

        result.ShouldHaveValidationErrorFor(b => b.CommissionPercentage)
            .WithErrorMessage("'Commission Percentage' must not be more than 10 digits in total, with allowance for 6 decimals. 9 digits and 8 decimals were found.");
    }
    private UpdateBrokerRequest ValidRequest() => new UpdateBrokerRequest
    {
        Code = "BRK-RO-001",
        CommissionPercentage = 0.0025M,
        Email = "john.doe@mail.com",
        Name = "John Doe",
        Phone = "0712345678",
    };
}
