using FluentValidation;
using FluentValidation.TestHelper;
using InsuranceApp.Application.Common.Repository;
using InsuranceApp.Application.Reports.DTOs;
using InsuranceApp.Application.Reports.Validators;
using InsuranceApp.Domain.Entities;
using InsuranceApp.Domain.Enums;
using Moq;
using System.Globalization;
using System.Linq.Expressions;

namespace InsuranceApp.UnitTests.Validators;

public class ReportFilterValidatorTests
{
    private readonly Mock<ICurrencyRepository> _currencyRepository = new();
    private readonly IValidator<ReportFilter> _validator;
    public ReportFilterValidatorTests()
    {
        _validator = new ReportFilterValidator(_currencyRepository.Object);
        _currencyRepository.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Currency, bool>>>(),
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
    public async Task ValidateAsync_FromDateInTheFuture_ShouldHaveError()
    {
        var req = ValidRequest();
        req.From = new DateOnly(9999, 01, 01);

        var result = await _validator.TestValidateAsync(req);

        result.ShouldHaveValidationErrorFor(b => b.From)
            .WithErrorMessage("From Date must be in the past.");
    }

    [Fact]
    public async Task ValidateAsync_ToDateBeforeFromDate_ShouldHaveError()
    {
        var req = ValidRequest();
        req.From = new DateOnly(2026, 04, 01);

        var result = await _validator.TestValidateAsync(req);

        result.ShouldHaveValidationErrorFor(b => b.To)
            .WithErrorMessage("From Date must be before the To Date.");
    }

    [Theory]
    [InlineData("EURO")]
    [InlineData("RO")]
    [InlineData("")]
    public async Task ValidateAsync_InvalidCurrencyCodeLength_ShouldHaveError(string currencyCode)
    {
        var req = ValidRequest();
        req.Currency = currencyCode;

        var result = await _validator.TestValidateAsync(req);

        result.ShouldHaveValidationErrorFor(b => b.Currency)
            .WithErrorMessage("Currency must have a length of 3 characters");
    }

    [Theory]
    [InlineData("123")]
    [InlineData("R1A")]
    [InlineData("AB1")]
    public async Task ValidateAsync_InvalidCurrencyCodeFormat_ShouldHaveError(string currencyCode)
    {
        var req = ValidRequest();
        req.Currency = currencyCode;

        var result = await _validator.TestValidateAsync(req);

        result.ShouldHaveValidationErrorFor(b => b.Currency)
            .WithErrorMessage("'Currency' is not in the correct format.");
    }

    [Fact]
    public async Task ValidateAsync_InvalidCurrencyCode_ShouldHaveError()
    {
        var req = ValidRequest();
        _currencyRepository.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Currency, bool>>>(),
            It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var result = await _validator.TestValidateAsync(req);

        result.ShouldHaveValidationErrorFor(b => b.Currency)
            .WithErrorMessage("Currency does not exist.");
    }

    private ReportFilter ValidRequest() => new ReportFilter()
    {
        From = new DateOnly(2019, 01, 01),
        To = new DateOnly(2020, 01, 01),
        Currency = "RON",
        Status = PolicyStatus.Active
    };
}
