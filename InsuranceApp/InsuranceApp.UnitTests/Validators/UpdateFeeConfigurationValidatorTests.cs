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

public class UpdateFeeConfigurationValidatorTests
{
    private readonly Mock<IRiskIndicatorRepository> _riskIndicatorRepository = new();
    private readonly IValidator<UpdateFeeConfigurationRequest> _validator;

    public UpdateFeeConfigurationValidatorTests()
    {
        _validator = new UpdateFeeConfigurationValidator(_riskIndicatorRepository.Object);
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

    [Fact]
    public async Task ValidateAsync_InvalidName_ShouldHaveError()
    {
        const string invalidName =
            "ManyCharactersManyCharactersManyCharactersManyCharactersManyCharactersManyCharactersManyCharactersManyCharacters" +
            "ManyCharactersManyCharactersManyCharactersManyCharactersManyCharactersManyCharactersManyCharactersManyCharactersMany" +
            "ManyCharactersManyCharactersManyCharactersManyCharactersManyCharactersManyCharactersManyCharactersManyCharacters";
        var req = ValidRequest();
        req.Name = invalidName;
        _riskIndicatorRepository.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<RiskIndicator, bool>>>(),
            It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var result = await _validator.TestValidateAsync(req);

        result.ShouldHaveValidationErrorFor(b => b.Name)
            .WithErrorMessage("Name must be between 1 and 256 characters.");
    }

    [Theory]
    [InlineData("Earthquake risk zone")]
    public async Task ValidateAsync_InvalidNameWhenRiskIndicatorType_ShouldHaveError(string name)
    {
        var req = ValidRequest();
        req.Name = name;
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
        req.Percentage = -1M;
        _riskIndicatorRepository.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<RiskIndicator, bool>>>(),
            It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var result = await _validator.TestValidateAsync(req);

        result.ShouldHaveValidationErrorFor(b => b.Percentage)
            .WithErrorMessage("Percentage must be greater than 0.");
    }

    private UpdateFeeConfigurationRequest ValidRequest() => new UpdateFeeConfigurationRequest
    {
        Type = FeeConfigurationType.RiskAdjustment,
        EffectiveFrom = DateTime.Parse("2021-01-01", CultureInfo.InvariantCulture),
        EffectiveTo = DateTime.Parse("2028-01-01", CultureInfo.InvariantCulture),
        IsActive = true,
        Name = "Earthquake risk zone adjustment",
        Percentage = 0.2000M
    };
}
