using AutoMapper;
using FluentAssertions;
using FluentValidation.Results;
using InsuranceApp.Application.Common.Errors;
using InsuranceApp.Application.Common.Pagination;
using InsuranceApp.Application.Common.Persistence;
using InsuranceApp.Application.Common.Repository;
using InsuranceApp.Application.Common.Validation;
using InsuranceApp.Application.Metadata.FeeConfigurations;
using InsuranceApp.Application.Metadata.FeeConfigurations.DTOs;
using InsuranceApp.Domain.Entities;
using InsuranceApp.Domain.Enums;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;

namespace InsuranceApp.UnitTests.Services;

public class FeeConfigurationServiceTests
{
    private readonly Mock<IFeeConfigurationRepository> _feeRepository = new();
    private readonly Mock<IRequestValidator> _requestValidator = new();
    private readonly Mock<IMapper> _mapper = new();
    private readonly Mock<ILogger<FeeConfigurationService>> _logger = new();
    private readonly IFeeConfigurationService _feeConfigurationService;

    public FeeConfigurationServiceTests()
    {
        _feeConfigurationService = new FeeConfigurationService(_feeRepository.Object,
            _requestValidator.Object, _mapper.Object, _logger.Object);
    }

    [Fact]
    public async Task ListAllFeeConfigurationsAsync_ShouldReturnPagedListOfFeeConfigurations()
    {
        var pageRequest = new PageRequest { PageNumber = 1, PageSize = 10 };
        var pagedFeeConfigurations = new PagedResult<FeeConfigurationDto>
        {
            Items = new List<FeeConfigurationDto>
            {
                new() { Type = FeeConfigurationType.RiskAdjustment, Name = "Earthquake risk zone adjustment", Percentage = 0.1M },
                new() { Type = FeeConfigurationType.RiskAdjustment, Name = "Flood zone adjustment", Percentage = 0.125M },
                new() { Type = FeeConfigurationType.BrokerCommission, Name = "Broker Commission", Percentage = 0.025M }
            },
            TotalCount = 3,
            PageNumber = 1,
            PageSize = 10
        };
        _feeRepository.Setup(r => r.GetAllAsync<FeeConfigurationDto>(pageRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedFeeConfigurations);

        var result = await _feeConfigurationService.ListAllFeeConfigurationsAsync(pageRequest, CancellationToken.None);

        result.IsSuccess.Should().BeTrue("A paged list of fee configurations should be returned in any case");
        result.Errors.Should().BeEmpty("There should be no errors when fetching a paged list of fee configurations");
        result.Value.Should().NotBeNull("A PagedResult should be returned");
        result.Value.Should().BeEquivalentTo(pagedFeeConfigurations);
        _feeRepository.Verify(r => r.GetAllAsync<FeeConfigurationDto>(pageRequest, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetFeeConfigurationByIdAsync_GivenExistingId_ShouldReturnFeeConfiguration()
    {
        var feeConfigurationId = Guid.NewGuid();
        var feeConfiguration = GetValidFeeConfiguration();
        feeConfiguration.Id = feeConfigurationId;
        var feeConfigurationDto = GetValidFeeConfigurationDto(feeConfiguration);
        _feeRepository.Setup(r => r.GetAsync(feeConfigurationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(feeConfiguration);
        _mapper.Setup(m => m.Map<FeeConfigurationDto>(feeConfiguration)).Returns(feeConfigurationDto);

        var result = await _feeConfigurationService.GetFeeConfigurationByIdAsync(feeConfigurationId, CancellationToken.None);

        result.IsSuccess.Should().BeTrue("A valid Fee Configuration Id should return a successful result");
        result.Errors.Should().BeEmpty("There should be no errors when passing an existing fee configuration id");
        result.Value.Should().NotBeNull("A FeeConfigurationDto should be returned");
        result.Value.Should().BeEquivalentTo(feeConfigurationDto);
        _mapper.Verify(m => m.Map<FeeConfigurationDto>(feeConfiguration), Times.Once);
        _feeRepository.Verify(r => r.GetAsync(feeConfigurationId, It.IsAny<CancellationToken>()), Times.Once);

    }

    [Fact]
    public async Task GetFeeConfigurationByIdAsync_GivenNonExistingId_ShouldReturnNotFound()
    {
        var feeConfigurationId = Guid.NewGuid();
        FeeConfiguration? feeConfiguration = null;
        _feeRepository.Setup(r => r.GetAsync(feeConfigurationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(feeConfiguration);

        var result = await _feeConfigurationService.GetFeeConfigurationByIdAsync(feeConfigurationId, CancellationToken.None);

        result.IsFailed.Should().BeTrue("An invalid Fee Configuration Id should return a fail result");
        result.Errors.Should().ContainSingle(e => e is NotFoundError
                                                  && e.Message == $"Fee Configuration '{feeConfigurationId}' not found.");
        _mapper.Verify(m => m.Map<FeeConfigurationDto>(feeConfiguration), Times.Never);
        _feeRepository.Verify(r => r.GetAsync(feeConfigurationId, It.IsAny<CancellationToken>()), Times.Once);

    }

    [Fact]
    public async Task CreateFeeConfigurationAsync_GivenValidRequest_ShouldReturnFeeConfigurationId()
    {
        var createRequest = GetValidCreateFeeConfigurationRequest();
        var feeConfiguration = GetValidFeeConfiguration();
        feeConfiguration.Type = createRequest.Type;
        feeConfiguration.Name = createRequest.Name;
        feeConfiguration.EffectiveFrom = createRequest.EffectiveFrom;
        feeConfiguration.IsActive = createRequest.IsActive;
        feeConfiguration.Percentage = createRequest.Percentage;
        _requestValidator.Setup(v => v.ValidateAsync(createRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _feeRepository.Setup(r =>
                r.ExistsAsync(It.IsAny<Expression<Func<FeeConfiguration, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _mapper.Setup(m => m.Map<FeeConfiguration>(createRequest)).Returns(feeConfiguration);

        var result = await _feeConfigurationService.CreateFeeConfigurationAsync(createRequest, CancellationToken.None);

        result.IsSuccess.Should().BeTrue("A valid request should return a successful result");
        result.Errors.Should().BeEmpty("There should be no errors when passing a valid request");
        result.Value.Should().Be(feeConfiguration.Id, "The fee configuration id should be returned");
        _requestValidator.Verify(v => v.ValidateAsync(createRequest, It.IsAny<CancellationToken>()), Times.Once);
        _feeRepository.Verify(r => r.ExistsAsync(It.IsAny<Expression<Func<FeeConfiguration, bool>>>(), It.IsAny<CancellationToken>()), Times.Once);
        _mapper.Verify(m => m.Map<FeeConfiguration>(createRequest), Times.Once);
        _feeRepository.Verify(r => r.AddAsync(feeConfiguration, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateFeeConfigurationAsync_GivenInvalidRequest_ShouldReturnValidationError()
    {
        var createRequest = new CreateFeeConfigurationRequest()
        {
            Name = "Broker Commission",
            EffectiveFrom = DateTime.UtcNow,
            IsActive = true,
            Percentage = 0.0025M
        };
        var failures = new[]
        {
            new ValidationFailure("Type", "Type is required and must be a valid value.")
            {
                ErrorCode = "TypeValidator"
            }
        };
        _requestValidator.Setup(v => v.ValidateAsync(createRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures));

        var result = await _feeConfigurationService.CreateFeeConfigurationAsync(createRequest, CancellationToken.None);

        result.IsFailed.Should().BeTrue("An invalid request should return a fail result");
        result.Errors.Should().ContainSingle(e => e is ValidationError);
        _requestValidator.Verify(v => v.ValidateAsync(createRequest, It.IsAny<CancellationToken>()), Times.Once);
        _feeRepository.Verify(r => r.ExistsAsync(It.IsAny<Expression<Func<FeeConfiguration, bool>>>(), It.IsAny<CancellationToken>()), Times.Never);
        _mapper.Verify(m => m.Map<FeeConfiguration>(createRequest), Times.Never);
        _feeRepository.Verify(r => r.AddAsync(It.IsAny<FeeConfiguration>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateFeeConfigurationAsync_GivenExistingName_ShouldReturnConflict()
    {
        var createRequest = new CreateFeeConfigurationRequest()
        {
            Type = FeeConfigurationType.BrokerCommission,
            Name = "Broker Commission",
            EffectiveFrom = DateTime.UtcNow,
            IsActive = true,
            Percentage = 0.0025M
        };
        _requestValidator.Setup(v => v.ValidateAsync(createRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _feeRepository.Setup(r =>
                r.ExistsAsync(It.IsAny<Expression<Func<FeeConfiguration, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _feeConfigurationService.CreateFeeConfigurationAsync(createRequest, CancellationToken.None);

        result.IsFailed.Should().BeTrue("A request with an already existing currency Code should return a fail result");
        result.Errors.Should().ContainSingle(e => e is ConflictError);
        _requestValidator.Verify(v => v.ValidateAsync(createRequest, It.IsAny<CancellationToken>()), Times.Once);
        _feeRepository.Verify(r => r.ExistsAsync(It.IsAny<Expression<Func<FeeConfiguration, bool>>>(), It.IsAny<CancellationToken>()), Times.Once);
        _mapper.Verify(m => m.Map<FeeConfiguration>(createRequest), Times.Never);
        _feeRepository.Verify(r => r.AddAsync(It.IsAny<FeeConfiguration>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateFeeConfigurationAsync_GivenExistingId_ShouldReturnFeeConfigurationId()
    {
        var feeConfigurationId = Guid.NewGuid();
        var updateRequest = new UpdateFeeConfigurationRequest()
        {
            Name = "New name",
            Percentage = 0.003M
        };
        var feeConfiguration = GetValidFeeConfiguration();
        feeConfiguration.Id = feeConfigurationId;
        _feeRepository.Setup(r => r.GetAsync(feeConfigurationId, It.IsAny<CancellationToken>())).ReturnsAsync(feeConfiguration);
        _requestValidator.Setup(v => v.ValidateAsync(updateRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _feeRepository.Setup(r =>
                r.ExistsAsync(It.IsAny<Expression<Func<FeeConfiguration, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _feeConfigurationService.UpdateFeeConfigurationAsync(feeConfigurationId, updateRequest, CancellationToken.None);

        result.IsSuccess.Should().BeTrue("A valid request should return a successful result");
        result.Errors.Should().BeEmpty("There should be no errors when passing a valid request");
        result.Value.Should().Be(feeConfiguration.Id, "The fee configuration id should be returned");
        _feeRepository.Verify(r => r.GetAsync(feeConfigurationId, It.IsAny<CancellationToken>()), Times.Once);
        _requestValidator.Verify(v => v.ValidateAsync(updateRequest, It.IsAny<CancellationToken>()), Times.Once);
        _feeRepository.Verify(r => r.ExistsAsync(It.IsAny<Expression<Func<FeeConfiguration, bool>>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateFeeConfigurationAsync_GivenNonExistingId_ShouldReturnNotFound()
    {
        var feeConfigurationId = Guid.NewGuid();
        var updateRequest = new UpdateFeeConfigurationRequest()
        {
            Percentage = 0.003M
        };
        FeeConfiguration? feeConfiguration = null;
        _feeRepository.Setup(r => r.GetAsync(feeConfigurationId, It.IsAny<CancellationToken>())).ReturnsAsync(feeConfiguration);

        var result = await _feeConfigurationService.UpdateFeeConfigurationAsync(feeConfigurationId, updateRequest, CancellationToken.None);

        result.IsFailed.Should().BeTrue("An invalid Fee Configuration Id should return a fail result");
        result.Errors.Should().ContainSingle(e => e is NotFoundError
                                                  && e.Message == $"Fee Configuration '{feeConfigurationId}' not found.");
        _feeRepository.Verify(r => r.GetAsync(feeConfigurationId, It.IsAny<CancellationToken>()), Times.Once);
        _requestValidator.Verify(v => v.ValidateAsync(updateRequest, It.IsAny<CancellationToken>()), Times.Never);
        _feeRepository.Verify(r => r.ExistsAsync(It.IsAny<Expression<Func<FeeConfiguration, bool>>>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateFeeConfigurationAsync_GivenInvalidRequest_ShouldReturnValidationError()
    {
        var feeConfigurationId = Guid.NewGuid();
        var updateRequest = new UpdateFeeConfigurationRequest()
        {
            Percentage = -0.1000M
        };
        var feeConfiguration = GetValidFeeConfiguration();
        feeConfiguration.Id = feeConfigurationId;
        var failures = new[]
        {
            new ValidationFailure("Percentage", "Percentage and must be greater than 0.")
            {
                ErrorCode = "PercentageValidator"
            }
        };
        _feeRepository.Setup(r => r.GetAsync(feeConfigurationId, It.IsAny<CancellationToken>())).ReturnsAsync(feeConfiguration);
        _requestValidator.Setup(v => v.ValidateAsync(updateRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures));

        var result = await _feeConfigurationService.UpdateFeeConfigurationAsync(feeConfigurationId, updateRequest, CancellationToken.None);

        result.IsFailed.Should().BeTrue("An invalid request should return a fail result");
        result.Errors.Should().ContainSingle(e => e is ValidationError);
        _feeRepository.Verify(r => r.GetAsync(feeConfigurationId, It.IsAny<CancellationToken>()), Times.Once);
        _requestValidator.Verify(v => v.ValidateAsync(updateRequest, It.IsAny<CancellationToken>()), Times.Once);
        _feeRepository.Verify(r => r.ExistsAsync(It.IsAny<Expression<Func<FeeConfiguration, bool>>>(), It.IsAny<CancellationToken>()), Times.Never);
    }


    [Fact]
    public async Task UpdateFeeConfigurationAsync_GivenExistingConfigurationName_ShouldReturnConflict()
    {
        var feeConfigurationId = Guid.NewGuid();
        var updateRequest = new UpdateFeeConfigurationRequest()
        {
            Name = "New Name",
            Percentage = 0.0025M
        };
        var feeConfiguration = GetValidFeeConfiguration();
        feeConfiguration.Id = feeConfigurationId;
        _feeRepository.Setup(r => r.GetAsync(feeConfigurationId, It.IsAny<CancellationToken>())).ReturnsAsync(feeConfiguration);
        _requestValidator.Setup(v => v.ValidateAsync(updateRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _feeRepository.Setup(r =>
                r.ExistsAsync(It.IsAny<Expression<Func<FeeConfiguration, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _feeConfigurationService.UpdateFeeConfigurationAsync(feeConfigurationId, updateRequest, CancellationToken.None);

        result.IsFailed.Should().BeTrue("A request with an already existing currency Code should return a fail result");
        result.Errors.Should().ContainSingle(e => e is ConflictError);
        _feeRepository.Verify(r => r.GetAsync(feeConfigurationId, It.IsAny<CancellationToken>()), Times.Once);
        _requestValidator.Verify(v => v.ValidateAsync(updateRequest, It.IsAny<CancellationToken>()), Times.Once);
        _feeRepository.Verify(r => r.ExistsAsync(It.IsAny<Expression<Func<FeeConfiguration, bool>>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    private CreateFeeConfigurationRequest GetValidCreateFeeConfigurationRequest() => new CreateFeeConfigurationRequest()
    {
        Type = FeeConfigurationType.BrokerCommission,
        Name = "Broker Commission",
        EffectiveFrom = DateTime.UtcNow,
        IsActive = true,
        Percentage = 0.0025M
    };

    private FeeConfiguration GetValidFeeConfiguration() => new FeeConfiguration()
    {
        Id = Guid.NewGuid(),
        Name = "Broker Commission",
        Type = FeeConfigurationType.BrokerCommission,
        Percentage = 0.002M,
        IsActive = true
    };

    private FeeConfigurationDto GetValidFeeConfigurationDto(FeeConfiguration feeConfiguration) =>
        new FeeConfigurationDto()
        {
            Id = feeConfiguration.Id,
            Name = feeConfiguration.Name,
            Percentage = feeConfiguration.Percentage,
            Type = feeConfiguration.Type,
            IsActive = feeConfiguration.IsActive,
            EffectiveFrom = feeConfiguration.EffectiveFrom,
            EffectiveTo = feeConfiguration.EffectiveTo
        };
}