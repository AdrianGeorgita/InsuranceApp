using System.Linq.Expressions;
using FluentValidation;
using FluentValidation.TestHelper;
using InsuranceApp.Application.Buildings.DTOs;
using InsuranceApp.Application.Buildings.Validators;
using InsuranceApp.Application.Common.Repository;
using InsuranceApp.Domain.Entities;
using InsuranceApp.Domain.Enums;
using Moq;

namespace InsuranceApp.UnitTests.Validators;

public class UpdateBuildingValidatorTests
{
    private readonly Mock<ICityRepository> _cityRepository = new();
    private readonly Mock<IRiskIndicatorRepository> _riskIndicatorRepository = new();
    private readonly IValidator<UpdateBuildingRequest> _validator;

    public UpdateBuildingValidatorTests()
    {
        _validator = new UpdateBuildingValidator(_riskIndicatorRepository.Object, _cityRepository.Object);
    }

    [Fact]
    public async Task ValidateAsync_RiskIndicatorIdsContainInvalidIds_ShouldHaveCustomError()
    {
        var req = ValidRequest();
        req.RiskIndicatorIds = new List<int>() { 1, 2, -5, 99, 99 };

        _cityRepository.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<City, bool>>>(), 
            It.IsAny<CancellationToken>())).ReturnsAsync(true);

        _riskIndicatorRepository.Setup(r => r.ListAllKeysAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<int> { 1, 2, 3 });

        var result = await _validator.TestValidateAsync(req);

        result.ShouldHaveValidationErrorFor("riskIndicatorIds")
            .WithErrorMessage("The following riskIndicatorIds do not exist: -5, 99");
    }

    [Fact]
    public async Task ValidateAsync_ValidRequest_ShouldNotHaveError()
    {
        var req = ValidRequest();

        _cityRepository.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<City, bool>>>(),
            It.IsAny<CancellationToken>())).ReturnsAsync(true);

        _riskIndicatorRepository.Setup(r => r.ListAllKeysAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<int> { 1, 2 });

        var result = await _validator.TestValidateAsync(req);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task ValidateAsync_NonExistingCityId_ShouldHaveError()
    {
        var req = ValidRequest();
        req.CityId = Guid.NewGuid();

        _cityRepository.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<City, bool>>>(),
            It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var result = await _validator.TestValidateAsync(req);

        result.ShouldHaveValidationErrorFor(b => b.CityId)
            .WithErrorMessage("City does not exist.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task ValidateAsync_InvalidInsuredValue_ShouldHaveError(decimal insuredValue)
    {
        var req = ValidRequest();
        req.InsuredValue = insuredValue;

        _cityRepository.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<City, bool>>>(),
            It.IsAny<CancellationToken>())).ReturnsAsync(true);

        _riskIndicatorRepository.Setup(r => r.ListAllKeysAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<int> { 1, 2 });

        var result = await _validator.TestValidateAsync(req);

        result.ShouldHaveValidationErrorFor(b => b.InsuredValue)
            .WithErrorMessage("InsuredValue must be greater than 0.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task ValidateAsync_InvalidSurfaceArea_ShouldHaveError(decimal surfaceArea)
    {
        var req = ValidRequest();
        req.SurfaceArea = surfaceArea;

        _cityRepository.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<City, bool>>>(),
            It.IsAny<CancellationToken>())).ReturnsAsync(true);

        _riskIndicatorRepository.Setup(r => r.ListAllKeysAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<int> { 1, 2 });

        var result = await _validator.TestValidateAsync(req);

        result.ShouldHaveValidationErrorFor(b => b.SurfaceArea)
            .WithErrorMessage("SurfaceArea must be greater than 0.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task ValidateAsync_InvalidNumberOfFloors_ShouldHaveError(int numberOfFloors)
    {
        var req = ValidRequest();
        req.NumberOfFloors = numberOfFloors;

        _cityRepository.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<City, bool>>>(),
            It.IsAny<CancellationToken>())).ReturnsAsync(true);

        _riskIndicatorRepository.Setup(r => r.ListAllKeysAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<int> { 1, 2 });

        var result = await _validator.TestValidateAsync(req);

        result.ShouldHaveValidationErrorFor(b => b.NumberOfFloors)
            .WithErrorMessage("NumberOfFloors must be greater than 0.");
    }

    [Theory]
    [InlineData("")]
    [InlineData("veryLongAddressveryLongAddressveryLongAddressveryLongAddressveryLongAddress" +
                "veryLongAddressveryLongAddressveryLongAddressveryLongAddressveryLongAddress" +
                "veryLongAddressveryLongAddressveryLongAddressveryLongAddressveryLongAddressveryLongAddress" +
                "veryLongAddressveryLongAddressveryLongAddressveryLongAddressveryLongAddress")]
    public async Task ValidateAsync_InvalidAddress_ShouldHaveError(string address)
    {
        var req = ValidRequest();
        req.Address = address;

        _cityRepository.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<City, bool>>>(),
            It.IsAny<CancellationToken>())).ReturnsAsync(true);

        _riskIndicatorRepository.Setup(r => r.ListAllKeysAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<int> { 1, 2 });

        var result = await _validator.TestValidateAsync(req);

        result.ShouldHaveValidationErrorFor(b => b.Address)
            .WithErrorMessage("Address Length must be between 1 and 255 characters.");
    }
    private UpdateBuildingRequest ValidRequest() => new UpdateBuildingRequest
    {
        Address = "Some Street",
        CityId = Guid.NewGuid(),
        ConstructionYear = 1980,
        BuildingType = BuildingType.Industrial,
        NumberOfFloors = 2,
        SurfaceArea = 125.20M,
        InsuredValue = 1003400,
        RiskIndicatorIds = new List<int> { 1, 2 }
    };
}
