using FluentValidation;
using FluentValidation.TestHelper;
using InsuranceApp.Application.Metadata.RiskFactorConfigurations.DTOs;
using InsuranceApp.Application.Metadata.RiskFactorConfigurations.Validators;
using InsuranceApp.Domain.Enums;

namespace InsuranceApp.UnitTests.Validators;

public class CreateRiskFactorConfigurationValidatorTests
{

    private readonly IValidator<CreateRiskFactorConfigurationRequest> _validator = new CreateRiskFactorConfigurationValidator();

    [Fact]
    public async Task ValidateAsync_ValidRequest_ShouldNotHaveError()
    {
        var req = ValidRequest();

        var result = await _validator.TestValidateAsync(req);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task ValidateAsync_MissingReferenceIdWhenGeographicLevel_ShouldHaveError()
    {
        var req = ValidRequest();
        req.ReferenceId = null;

        var result = await _validator.TestValidateAsync(req);

        result.ShouldHaveValidationErrorFor(b => b.ReferenceId)
            .WithErrorMessage("ReferenceId is required for Geographic Configurations and must be a valid guid.");
    }

    [Fact]
    public async Task ValidateAsync_InvalidExchangeRateToBase_ShouldHaveError()
    {
        var req = ValidRequest();
        req.AdjustmentPercentage = 0M;

        var result = await _validator.TestValidateAsync(req);

        result.ShouldHaveValidationErrorFor(b => b.AdjustmentPercentage)
            .WithErrorMessage("AdjustmentPercentage is required and must be a valid decimal number.");
    }
    private CreateRiskFactorConfigurationRequest ValidRequest() => new CreateRiskFactorConfigurationRequest()
    {
        Level = RiskFactorConfigurationLevel.City,
        ReferenceId = Guid.NewGuid(),
        AdjustmentPercentage = 0.1234M,
        IsActive = true
    };
}
