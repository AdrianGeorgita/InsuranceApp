using System.Globalization;
using FluentValidation;
using FluentValidation.TestHelper;
using InsuranceApp.Application.Metadata.Currencies.DTOs;
using InsuranceApp.Application.Metadata.Currencies.Validators;

namespace InsuranceApp.UnitTests.Validators;

public class CreateCurrencyValidatorTests
{

    private readonly IValidator<CreateCurrencyRequest> _validator = new CreateCurrencyValidator();

    [Fact]
    public async Task ValidateAsync_ValidRequest_ShouldNotHaveError()
    {
        var req = ValidRequest();

        var result = await _validator.TestValidateAsync(req);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("aaaa")]
    [InlineData("a")]
    [InlineData("")]
    public async Task ValidateAsync_InvalidCode_ShouldHaveError(string code)
    {
        var req = ValidRequest();
        req.Code = code;

        var result = await _validator.TestValidateAsync(req);

        result.ShouldHaveValidationErrorFor(b => b.Code)
            .WithErrorMessage("Currency Code is required and must be of exactly 3 characters.");
    }

    [Theory]
    [InlineData("ManyCharactersManyCharactersManyCharactersManyCharactersManyCharactersManyCharactersManyCharactersManyCharacters" +
                "ManyCharactersManyCharactersManyCharactersManyCharactersManyCharactersManyCharactersManyCharactersManyCharactersMany" +
                "ManyCharactersManyCharactersManyCharactersManyCharactersManyCharactersManyCharactersManyCharactersManyCharacters")]
    [InlineData("")]
    public async Task ValidateAsync_InvalidName_ShouldHaveError(string name)
    {
        var req = ValidRequest();
        req.Name = name;

        var result = await _validator.TestValidateAsync(req);

        result.ShouldHaveValidationErrorFor(b => b.Name)
            .WithErrorMessage("Name is required and must be between 1 and 256 characters.");
    }


    [Theory]
    [InlineData("0")]
    [InlineData("-1")]
    public async Task ValidateAsync_InvalidExchangeRateToBase_ShouldHaveError(string exchangeRateToBase)
    {
        var rate = decimal.Parse(
            exchangeRateToBase,
            CultureInfo.InvariantCulture);
        var req = ValidRequest();
        req.ExchangeRateToBase = rate;

        var result = await _validator.TestValidateAsync(req);

        result.ShouldHaveValidationErrorFor(b => b.ExchangeRateToBase)
            .WithErrorMessage("ExchangeRateToBase is required and must be greater than 0.");
    }

    [Fact]
    public async Task ValidateAsync_InvalidExchangeRateToBasePrecision_ShouldHaveError()
    {
        var req = ValidRequest();
        req.ExchangeRateToBase = 0.32131233M;

        var result = await _validator.TestValidateAsync(req);

        result.ShouldHaveValidationErrorFor(b => b.ExchangeRateToBase)
            .WithErrorMessage("'Exchange Rate To Base' must not be more than 10 digits in total, with allowance for 6 decimals. 9 digits and 8 decimals were found.");
    }
    private CreateCurrencyRequest ValidRequest() => new CreateCurrencyRequest()
    {
        Code = "RON",
        Name = "Romanian Leu",
        ExchangeRateToBase = 1.0000M,
        IsActive = true
    };
}
