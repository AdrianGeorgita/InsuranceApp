using System.Globalization;
using System.Linq.Expressions;
using FluentValidation;
using FluentValidation.TestHelper;
using InsuranceApp.Application.Common.Repository;
using InsuranceApp.Application.Policies.DTOs;
using InsuranceApp.Application.Policies.Validators;
using InsuranceApp.Domain.Entities;
using Moq;

namespace InsuranceApp.UnitTests.Validators;

public class CreatePolicyValidatorTests
{
    private readonly Mock<IClientRepository> _clientRepository = new();
    private readonly Mock<IBuildingRepository> _buildingRepository = new();
    private readonly Mock<ICurrencyRepository> _currencyRepository = new();
    private readonly Mock<IBrokerRepository> _brokerRepository = new();
    private readonly IValidator<CreatePolicyRequest> _validator;
    public CreatePolicyValidatorTests()
    {
        _validator = new CreatePolicyValidator(
            _clientRepository.Object, _buildingRepository.Object,
            _currencyRepository.Object, _brokerRepository.Object);

        _clientRepository.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Client, bool>>>(),
            It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _buildingRepository.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Building, bool>>>(),
            It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _currencyRepository.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Currency, bool>>>(),
            It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _brokerRepository.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Broker, bool>>>(),
            It.IsAny<CancellationToken>())).ReturnsAsync(true);
    }

    [Fact]
    public async Task ValidateAsync_ValidRequest_ShouldNotHaveError()
    {
        var req = ValidRequest();

        var result = await _validator.TestValidateAsync(req);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task ValidateAsync_InvalidClientId_ShouldHaveError()
    {
        var req = ValidRequest();
        _clientRepository.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Client, bool>>>(),
            It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var result = await _validator.TestValidateAsync(req);

        result.ShouldHaveValidationErrorFor(b => b.ClientId)
            .WithErrorMessage("Client does not exist.");
    }

    [Fact]
    public async Task ValidateAsync_InvalidBuildingId_ShouldHaveError()
    {
        var req = ValidRequest();
        _buildingRepository.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Building, bool>>>(),
            It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var result = await _validator.TestValidateAsync(req);

        result.ShouldHaveValidationErrorFor(b => b.BuildingId)
            .WithErrorMessage("Building does not exist.");
    }

    [Theory]
    [InlineData("2021-01-01")]
    [InlineData("0001-01-01")]
    public async Task ValidateAsync_InvalidStartDate_ShouldHaveError(string startDate)
    {
        var req = ValidRequest();
        req.StartDate = DateTime.Parse(startDate, CultureInfo.InvariantCulture);

        var result = await _validator.TestValidateAsync(req);

        result.ShouldHaveValidationErrorFor(b => b.StartDate)
            .WithErrorMessage("StartDate is required and cannot be in the past.");
    }

    [Theory]
    [InlineData("2021-01-01")]
    [InlineData("0001-01-01")]
    public async Task ValidateAsync_InvalidEndDate_ShouldHaveError(string endDate)
    {
        var req = ValidRequest();
        req.EndDate = DateTime.Parse(endDate, CultureInfo.InvariantCulture);

        var result = await _validator.TestValidateAsync(req);

        result.ShouldHaveValidationErrorFor(b => b.EndDate)
            .WithErrorMessage("EndDate is required and cannot be in the past.");
    }

    [Fact]
    public async Task ValidateAsync_EndDateBeforeStartDate_ShouldHaveError()
    {
        var req = ValidRequest();
        req.EndDate = new DateTime(2026, 04, 01);

        var result = await _validator.TestValidateAsync(req);

        result.ShouldHaveValidationErrorFor(b => b.EndDate)
            .WithErrorMessage("EndDate must be after the StartDate.");
    }

    [Theory]
    [InlineData("-1.0")]
    [InlineData("0.0")]
    public async Task ValidateAsync_InvalidBasePremiumValue_ShouldHaveError(string basePremium)
    {
        var premium = decimal.Parse(
            basePremium,
            CultureInfo.InvariantCulture);
        var req = ValidRequest();
        req.BasePremium = premium;

        var result = await _validator.TestValidateAsync(req);

        result.ShouldHaveValidationErrorFor(b => b.BasePremium)
            .WithErrorMessage("BasePremium is required and must be greater than 0.");
    }

    [Theory]
    [InlineData("EURO")]
    [InlineData("RO")]
    [InlineData("")]
    public async Task ValidateAsync_InvalidCurrencyCodeLength_ShouldHaveError(string currencyCode)
    {
        var req = ValidRequest();
        req.CurrencyCode = currencyCode;

        var result = await _validator.TestValidateAsync(req);

        result.ShouldHaveValidationErrorFor(b => b.CurrencyCode)
            .WithErrorMessage("CurrencyCode is required and must have a length of 3 characters");
    }

    [Theory]
    [InlineData("123")]
    [InlineData("R1A")]
    [InlineData("AB1")]
    public async Task ValidateAsync_InvalidCurrencyCodeFormat_ShouldHaveError(string currencyCode)
    {
        var req = ValidRequest();
        req.CurrencyCode = currencyCode;

        var result = await _validator.TestValidateAsync(req);

        result.ShouldHaveValidationErrorFor(b => b.CurrencyCode)
            .WithErrorMessage("'Currency Code' is not in the correct format.");
    }

    [Fact]
    public async Task ValidateAsync_InvalidCurrencyCode_ShouldHaveError()
    {
        var req = ValidRequest();
        _currencyRepository.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Currency, bool>>>(),
            It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var result = await _validator.TestValidateAsync(req);

        result.ShouldHaveValidationErrorFor(b => b.CurrencyCode)
            .WithErrorMessage("Currency does not exist or is deprecated.");
    }

    [Fact]
    public async Task ValidateAsync_InvalidBrokerId_ShouldHaveError()
    {
        var req = ValidRequest();
        _brokerRepository.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Broker, bool>>>(),
            It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var result = await _validator.TestValidateAsync(req);

        result.ShouldHaveValidationErrorFor(b => b.BrokerId)
            .WithErrorMessage("Broker does not exist or is inactive.");
    }
    private CreatePolicyRequest ValidRequest() => new CreatePolicyRequest
    {
        ClientId = Guid.NewGuid(),
        BuildingId = Guid.NewGuid(),
        BasePremium = 10000M,
        BrokerId = Guid.NewGuid(),
        CurrencyCode = "RON",
        StartDate = new DateTime(2026, 05, 11),
        EndDate = new DateTime(2027, 05, 11)
    };
}
