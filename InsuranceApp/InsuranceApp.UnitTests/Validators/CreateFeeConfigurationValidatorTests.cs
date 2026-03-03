using System.Globalization;
using System.Linq.Expressions;
using FluentValidation;
using FluentValidation.TestHelper;
using InsuranceApp.Application.Common.Repository;
using InsuranceApp.Application.Metadata.FeeConfigurations.DTOs;
using InsuranceApp.Application.Metadata.FeeConfigurations.Validators;
using InsuranceApp.Domain.Entities;
using InsuranceApp.Domain.Enums;
using Moq;

namespace InsuranceApp.UnitTests.Validators;

public class CreateFeeConfigurationValidatorTests
{
    private readonly Mock<IRiskIndicatorRepository> _riskIndicatorRepository = new();
    private readonly IValidator<CreateFeeConfigurationRequest> _validator;

    public CreateFeeConfigurationValidatorTests()
    {
        _validator = new CreateFeeConfigurationValidator(_riskIndicatorRepository.Object);
    }

    [Fact]
    public async Task ValidateAsync_ValidRequest_ShouldNotHaveError()
    {
        var req = ValidRequest();
        _riskIndicatorRepository.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<RiskIndicator, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _validator.TestValidateAsync(req);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task ValidateAsync_NameDoesNotContainValidRiskIndicator_ShouldHaveCustomError()
    {
        var req = ValidRequest();
        req.Name = "Earthquake adjustment";
        _riskIndicatorRepository.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<RiskIndicator, bool>>>(),
            It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var result = await _validator.TestValidateAsync(req);

        result.ShouldHaveValidationErrorFor("Name")
            .WithErrorMessage("Risk Indicator does not exist.");
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
        _riskIndicatorRepository.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<RiskIndicator, bool>>>(),
            It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var result = await _validator.TestValidateAsync(req);

        result.ShouldHaveValidationErrorFor(b => b.Name)
            .WithErrorMessage("Name is required and must be between 1 and 256 characters.");
    }

    [Fact]
    public async Task ValidateAsync_InvalidNameWhenRiskIndicatorType_ShouldHaveError()
    {
        const string invalidRiskIndicatorName = "Earthquake risk zone";
        var req = ValidRequest();
        req.Name = invalidRiskIndicatorName;
        _riskIndicatorRepository.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<RiskIndicator, bool>>>(),
            It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var result = await _validator.TestValidateAsync(req);

        result.ShouldHaveValidationErrorFor(b => b.Name)
            .WithErrorMessage("Name must end with 'adjustment' when adding a RiskAdjustment.");
    }

    [Fact]
    public async Task ValidateAsync_InvalidPercentage_ShouldHaveError()
    {
        var req = ValidRequest();
        req.Percentage = 0M;
        _riskIndicatorRepository.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<RiskIndicator, bool>>>(),
            It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var result = await _validator.TestValidateAsync(req);

        result.ShouldHaveValidationErrorFor(b => b.Percentage)
            .WithErrorMessage("Percentage is required and must be greater than 0.");
    }

    [Fact]
    public async Task ValidateAsync_EffectiveToDateBeforeEffectiveFromDate_ShouldHaveError()
    {
        var req = ValidRequest();
        req.EffectiveFrom = DateTime.Parse("2029-01-01", CultureInfo.InvariantCulture);
        req.EffectiveTo = DateTime.Parse("2028-01-01", CultureInfo.InvariantCulture);
        _riskIndicatorRepository.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<RiskIndicator, bool>>>(),
            It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var result = await _validator.TestValidateAsync(req);

        result.ShouldHaveValidationErrorFor(b => b.EffectiveTo)
            .WithErrorMessage("EffectiveTo must be a date after the EffectiveFrom date.");
    }
    private CreateFeeConfigurationRequest ValidRequest() => new CreateFeeConfigurationRequest
    {
        Type = FeeConfigurationType.RiskAdjustment,
        EffectiveFrom = DateTime.Parse("2021-01-01", CultureInfo.InvariantCulture),
        EffectiveTo = DateTime.Parse("2028-01-01", CultureInfo.InvariantCulture),
        IsActive = true,
        Name = "Earthquake risk zone adjustment",
        Percentage = 0.2000M
    };
}
