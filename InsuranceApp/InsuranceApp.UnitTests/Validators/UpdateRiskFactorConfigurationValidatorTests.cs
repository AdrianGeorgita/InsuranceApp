using FluentValidation;
using FluentValidation.TestHelper;
using InsuranceApp.Application.Metadata.RiskFactorConfigurations.DTOs;
using InsuranceApp.Application.Metadata.RiskFactorConfigurations.Validators;
using InsuranceApp.Domain.Enums;

namespace InsuranceApp.UnitTests.Validators;

public class UpdateRiskFactorConfigurationValidatorTests
{

    private readonly IValidator<UpdateRiskFactorConfigurationRequest> _validator = new UpdateRiskFactorConfigurationValidator();

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

    private UpdateRiskFactorConfigurationRequest ValidRequest() => new UpdateRiskFactorConfigurationRequest()
    {
        Level = RiskFactorConfigurationLevel.City,
        ReferenceId = Guid.NewGuid(),
        AdjustmentPercentage = 0.1234M,
        IsActive = true
    };
}
