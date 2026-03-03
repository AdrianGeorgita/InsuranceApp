using AutoMapper;
using FluentAssertions;
using FluentValidation.Results;
using InsuranceApp.Application.Common.Errors;
using InsuranceApp.Application.Common.Pagination;
using InsuranceApp.Application.Common.Repository;
using InsuranceApp.Application.Common.Validation;
using InsuranceApp.Application.Metadata.Currencies;
using InsuranceApp.Application.Metadata.Currencies.DTOs;
using InsuranceApp.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;

namespace InsuranceApp.UnitTests.Services;

public class CurrencyServiceTests
{
    private readonly Mock<ICurrencyRepository> _currencyRepository = new();
    private readonly Mock<IPolicyRepository> _policyRepository = new();
    private readonly Mock<IRequestValidator> _requestValidator = new();
    private readonly Mock<IMapper> _mapper = new();
    private readonly Mock<ILogger<CurrencyService>> _logger = new();
    private readonly ICurrencyService _currencyService;

    public CurrencyServiceTests()
    {
        _currencyService = new CurrencyService(_currencyRepository.Object, _policyRepository.Object,
            _requestValidator.Object, _mapper.Object, _logger.Object);
    }

    [Fact]
    public async Task ListAllCurrenciesAsync_ShouldReturnPagedListOfCurrencies()
    {
        var pageRequest = new PageRequest { PageNumber = 1, PageSize = 10 };
        var pagedCurrencies = new PagedResult<CurrencyDto>
        {
            Items = new List<CurrencyDto>
            {
                new() { Code = "RON", Name = "Romanian Leu", ExchangeRateToBase = 1.0000M, IsActive = true},
                new() { Code = "USD", Name = "US Dollar", ExchangeRateToBase = 4.5800M, IsActive = true},
                new() { Code = "GBP", Name = "British Pound", ExchangeRateToBase = 5.8200M, IsActive = true}
            },
            TotalCount = 3,
            PageNumber = 1,
            PageSize = 10
        };
        _currencyRepository.Setup(r => r.GetAllAsync<CurrencyDto>(pageRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedCurrencies);

        var result = await _currencyService.ListAllCurrenciesAsync(pageRequest, CancellationToken.None);

        result.IsSuccess.Should().BeTrue("A paged list of currencies should be returned in any case");
        result.Errors.Should().BeEmpty("There should be no errors when fetching a paged list of currencies");
        result.Value.Should().NotBeNull("A PagedResult should be returned");
        result.Value.Should().BeEquivalentTo(pagedCurrencies);
        _currencyRepository.Verify(r => r.GetAllAsync<CurrencyDto>(pageRequest, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetCurrencyByCodeAsync_GivenExistingCurrencyCode_ShouldReturnCurrency()
    {
        const string currencyCode = "RON";
        var currency = GetValidCurrency();
        currency.Code = currencyCode;
        var currencyDto = new CurrencyDto
        {
            Code = currency.Code,
            Name = currency.Name,
            ExchangeRateToBase = currency.ExchangeRateToBase,
            IsActive = currency.IsActive
        };
        _currencyRepository.Setup(r => r.GetAsync(currencyCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync(currency);
        _mapper.Setup(m => m.Map<CurrencyDto>(currency)).Returns(currencyDto);

        var result = await _currencyService.GetCurrencyByIdAsync(currencyCode, CancellationToken.None);

        result.IsSuccess.Should().BeTrue("A valid CurrencyCode should return a successful result");
        result.Errors.Should().BeEmpty("There should be no errors when passing an existing currency code");
        result.Value.Should().NotBeNull("A CurrencyDto should be returned");
        result.Value.Should().BeEquivalentTo(currencyDto);
        _mapper.Verify(m => m.Map<CurrencyDto>(currency), Times.Once);
        _currencyRepository.Verify(r => r.GetAsync(currencyCode, It.IsAny<CancellationToken>()), Times.Once);

    }

    [Fact]
    public async Task GetCurrencyByCodeAsync_GivenNonExistingCurrencyCode_ShouldReturnNotFound()
    {
        const string currencyCode = "RON";
        Currency? currency = null;
        _currencyRepository.Setup(r => r.GetAsync(currencyCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync(currency);

        var result = await _currencyService.GetCurrencyByIdAsync(currencyCode, CancellationToken.None);

        result.IsFailed.Should().BeTrue("An invalid CurrencyCode should return a fail result");
        result.Errors.Should().ContainSingle(e => e is NotFoundError
                                                  && e.Message == $"Currency '{currencyCode}' not found.");
        _mapper.Verify(m => m.Map<CurrencyDto>(currency), Times.Never);
        _currencyRepository.Verify(r => r.GetAsync(currencyCode, It.IsAny<CancellationToken>()), Times.Once);

    }

    [Fact]
    public async Task CreateCurrencyAsync_GivenValidRequest_ShouldReturnCurrencyCode()
    {
        var createRequest = GetValidCreateCurrencyRequest();
        var currency = GetValidCurrency();
        currency.Code = createRequest.Code;
        currency.ExchangeRateToBase = createRequest.ExchangeRateToBase;
        currency.Name = createRequest.Name;
        currency.IsActive = createRequest.IsActive;
        _requestValidator.Setup(v => v.ValidateAsync(createRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _currencyRepository.Setup(r =>
                r.ExistsByCodeAsync(createRequest.Code, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _mapper.Setup(m => m.Map<Currency>(createRequest)).Returns(currency);

        var result = await _currencyService.CreateCurrencyAsync(createRequest, CancellationToken.None);

        result.IsSuccess.Should().BeTrue("A valid request should return a successful result");
        result.Errors.Should().BeEmpty("There should be no errors when passing a valid request");
        result.Value.Should().Be(currency.Code, "The currency code should be returned");
        _requestValidator.Verify(v => v.ValidateAsync(createRequest, It.IsAny<CancellationToken>()), Times.Once);
        _currencyRepository.Verify(r => r.ExistsByCodeAsync(createRequest.Code, It.IsAny<CancellationToken>()), Times.Once);
        _mapper.Verify(m => m.Map<Currency>(createRequest), Times.Once);
        _currencyRepository.Verify(r => r.AddAsync(currency, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateCurrencyAsync_GivenInvalidRequest_ShouldReturnValidationError()
    {
        var createRequest = GetValidCreateCurrencyRequest();
        createRequest.Code = "RO";
        var failures = new[]
        {
            new ValidationFailure("Code", "Currency Code is required and must be of exactly 3 characters.")
            {
                ErrorCode = "CodeValidator"
            }
        };
        _requestValidator.Setup(v => v.ValidateAsync(createRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures));

        var result = await _currencyService.CreateCurrencyAsync(createRequest, CancellationToken.None);

        result.IsFailed.Should().BeTrue("An invalid request should return a fail result");
        result.Errors.Should().ContainSingle(e => e is ValidationError);
        _requestValidator.Verify(v => v.ValidateAsync(createRequest, It.IsAny<CancellationToken>()), Times.Once);
        _currencyRepository.Verify(r => r.ExistsByCodeAsync(createRequest.Code, It.IsAny<CancellationToken>()), Times.Never);
        _mapper.Verify(m => m.Map<Currency>(createRequest), Times.Never);
        _currencyRepository.Verify(r => r.AddAsync(It.IsAny<Currency>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateCurrencyAsync_GivenExistingCurrencyCode_ShouldReturnConflict()
    {
        var createRequest = GetValidCreateCurrencyRequest();
        _requestValidator.Setup(v => v.ValidateAsync(createRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _currencyRepository.Setup(r =>
                r.ExistsByCodeAsync(createRequest.Code, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _currencyService.CreateCurrencyAsync(createRequest, CancellationToken.None);

        result.IsFailed.Should().BeTrue("A request with an already existing currency Code should return a fail result");
        result.Errors.Should().ContainSingle(e => e is ConflictError);
        _requestValidator.Verify(v => v.ValidateAsync(createRequest, It.IsAny<CancellationToken>()), Times.Once);
        _currencyRepository.Verify(r => r.ExistsByCodeAsync(createRequest.Code, It.IsAny<CancellationToken>()), Times.Once);
        _mapper.Verify(m => m.Map<Currency>(createRequest), Times.Never);
        _currencyRepository.Verify(r => r.AddAsync(It.IsAny<Currency>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateCurrencyAsync_GivenExistingCurrencyCode_ShouldReturnCurrencyCode()
    {
        const string currencyCode = "EUR";
        var updateRequest = new UpdateCurrencyRequest()
        {
            ExchangeRateToBase = 5.0123M,
            IsActive = false
        };
        var currency = GetValidCurrency();
        currency.Code = currencyCode;
        _currencyRepository.Setup(r => r.GetAsync(currencyCode, It.IsAny<CancellationToken>())).ReturnsAsync(currency);
        _requestValidator.Setup(v => v.ValidateAsync(updateRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _currencyRepository.Setup(r =>
                r.ExistsByCodeAsync(null!, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _currencyService.UpdateCurrencyAsync(currencyCode, updateRequest, CancellationToken.None);

        result.IsSuccess.Should().BeTrue("A valid request should return a successful result");
        result.Errors.Should().BeEmpty("There should be no errors when passing a valid request");
        result.Value.Should().Be(currency.Code, "The currency code should be returned");
        _currencyRepository.Verify(r => r.GetAsync(currencyCode, It.IsAny<CancellationToken>()), Times.Once);
        _requestValidator.Verify(v => v.ValidateAsync(updateRequest, It.IsAny<CancellationToken>()), Times.Once);
        _currencyRepository.Verify(r => r.ExistsByCodeAsync(null!, It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateCurrencyAsync_GivenNonExistingCurrencyCode_ShouldReturnNotFound()
    {
        const string currencyCode = "EUR";
        var updateRequest = new UpdateCurrencyRequest()
        {
            ExchangeRateToBase = 5.0123M,
            IsActive = false
        };
        Currency? currency = null;
        _currencyRepository.Setup(r => r.GetAsync(currencyCode, It.IsAny<CancellationToken>())).ReturnsAsync(currency);

        var result = await _currencyService.UpdateCurrencyAsync(currencyCode, updateRequest, CancellationToken.None);

        result.IsFailed.Should().BeTrue("An invalid CurrencyCode should return a fail result");
        result.Errors.Should().ContainSingle(e => e is NotFoundError
                                                  && e.Message == $"Currency '{currencyCode}' not found.");
        _currencyRepository.Verify(r => r.GetAsync(currencyCode, It.IsAny<CancellationToken>()), Times.Once);
        _requestValidator.Verify(v => v.ValidateAsync(updateRequest, It.IsAny<CancellationToken>()), Times.Never);
        _currencyRepository.Verify(r => r.ExistsByCodeAsync(null!, It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateCurrencyAsync_GivenInvalidRequest_ShouldReturnValidationError()
    {
        const string currencyCode = "EUR";
        var updateRequest = new UpdateCurrencyRequest()
        {
            ExchangeRateToBase = -5.0123M,
            IsActive = false
        };
        var currency = GetValidCurrency();
        currency.Code = currencyCode;
        var failures = new[]
        {
            new ValidationFailure("ExchangeRateToBase", "ExchangeRateToBase must be greater than 0.")
            {
                ErrorCode = "ExchangeRateToBaseValidator"
            }
        };
        _currencyRepository.Setup(r => r.GetAsync(currencyCode, It.IsAny<CancellationToken>())).ReturnsAsync(currency);
        _requestValidator.Setup(v => v.ValidateAsync(updateRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures));

        var result = await _currencyService.UpdateCurrencyAsync(currencyCode, updateRequest, CancellationToken.None);

        result.IsFailed.Should().BeTrue("An invalid request should return a fail result");
        result.Errors.Should().ContainSingle(e => e is ValidationError);
        _currencyRepository.Verify(r => r.GetAsync(currencyCode, It.IsAny<CancellationToken>()), Times.Once);
        _requestValidator.Verify(v => v.ValidateAsync(updateRequest, It.IsAny<CancellationToken>()), Times.Once);
        _currencyRepository.Verify(r => r.ExistsByCodeAsync(null!, It.IsAny<CancellationToken>()), Times.Never);
    }


    [Fact]
    public async Task UpdateCurrencyAsync_GivenExistingNewCurrencyCode_ShouldReturnConflict()
    {
        const string currencyCode = "EUR";
        var updateRequest = new UpdateCurrencyRequest()
        {
            Code = "EUZ",
            ExchangeRateToBase = 5.0123M,
            IsActive = false
        };
        var currency = GetValidCurrency();
        currency.Code = currencyCode;
        _currencyRepository.Setup(r => r.GetAsync(currencyCode, It.IsAny<CancellationToken>())).ReturnsAsync(currency);
        _requestValidator.Setup(v => v.ValidateAsync(updateRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _currencyRepository.Setup(r =>
                r.ExistsByCodeAsync(updateRequest.Code, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _currencyService.UpdateCurrencyAsync(currencyCode, updateRequest, CancellationToken.None);

        result.IsFailed.Should().BeTrue("A request with an already existing currency Code should return a fail result");
        result.Errors.Should().ContainSingle(e => e is ConflictError);
        _currencyRepository.Verify(r => r.GetAsync(currencyCode, It.IsAny<CancellationToken>()), Times.Once);
        _requestValidator.Verify(v => v.ValidateAsync(updateRequest, It.IsAny<CancellationToken>()), Times.Once);
        _currencyRepository.Verify(r => r.ExistsByCodeAsync(updateRequest.Code, It.IsAny<CancellationToken>()), Times.Once);
    }

    private Currency GetValidCurrency() => new Currency()
    {
        Code = "EUR",
        ExchangeRateToBase = 5.0110M,
        Name = "Euro",
        IsActive = true
    };

    private CreateCurrencyRequest GetValidCreateCurrencyRequest() => new CreateCurrencyRequest()
    {
        Code = "RON",
        Name = "Romanian Leu",
        ExchangeRateToBase = 1.0000M,
        IsActive = true
    };
}