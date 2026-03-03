using AutoMapper;
using FluentAssertions;
using FluentValidation.Results;
using InsuranceApp.Application.Common.Errors;
using InsuranceApp.Application.Common.Pagination;
using InsuranceApp.Application.Common.Repository;
using InsuranceApp.Application.Common.Validation;
using InsuranceApp.Application.Metadata.RiskFactorConfigurations;
using InsuranceApp.Application.Metadata.RiskFactorConfigurations.DTOs;
using InsuranceApp.Domain.Entities;
using InsuranceApp.Domain.Enums;
using Microsoft.Extensions.Logging;
using Moq;

namespace InsuranceApp.UnitTests.Services;

public class RiskFactorConfigurationServiceTests
{
    private readonly Mock<IRiskFactorConfigurationRepository> _riskFactorRepository = new();
    private readonly Mock<IRequestValidator> _requestValidator = new();
    private readonly Mock<IMapper> _mapper = new();
    private readonly Mock<ILogger<RiskFactorConfigurationService>> _logger = new();
    private readonly IRiskFactorConfigurationService _riskFactorConfigurationService;

    public RiskFactorConfigurationServiceTests()
    {
        _riskFactorConfigurationService = new RiskFactorConfigurationService(_riskFactorRepository.Object,
            _requestValidator.Object, _mapper.Object, _logger.Object);
    }

    [Fact]
    public async Task ListAllRiskFactorConfigurationAsync_ShouldReturnPagedListOfRiskFactorConfigurations()
    {
        var pageRequest = new PageRequest { PageNumber = 1, PageSize = 10 };
        var pagedRiskFactorConfigurations = new PagedResult<RiskFactorConfigurationDto>
        {
            Items = new List<RiskFactorConfigurationDto>
            {
                new() { Level = RiskFactorConfigurationLevel.City, ReferenceId = Guid.NewGuid(), AdjustmentPercentage = 0.025M},
                new() { Level = RiskFactorConfigurationLevel.County, ReferenceId = Guid.NewGuid(), AdjustmentPercentage = 0.030M},
                new() { Level = RiskFactorConfigurationLevel.BuildingType, BuildingType = BuildingType.Office, AdjustmentPercentage = -0.015M},
            },
            TotalCount = 3,
            PageNumber = 1,
            PageSize = 10
        };
        _riskFactorRepository.Setup(r => r.GetAllAsync<RiskFactorConfigurationDto>(pageRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedRiskFactorConfigurations);

        var result = await _riskFactorConfigurationService.ListAllRiskFactorConfigurationAsync(pageRequest, CancellationToken.None);

        result.IsSuccess.Should().BeTrue("A paged list of risk factor configurations should be returned in any case");
        result.Errors.Should().BeEmpty("There should be no errors when fetching a paged list of risk factor configurations");
        result.Value.Should().NotBeNull("A PagedResult should be returned");
        result.Value.Should().BeEquivalentTo(pagedRiskFactorConfigurations);
        _riskFactorRepository.Verify(r => r.GetAllAsync<RiskFactorConfigurationDto>(pageRequest, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetRiskFactorConfigurationByIdAsync_GivenExistingId_ShouldReturnRiskFactorConfiguration()
    {
        var riskFactorConfigurationId = Guid.NewGuid();
        var riskFactorConfiguration = new RiskFactorConfiguration
        {
            Id = riskFactorConfigurationId,
            Level = RiskFactorConfigurationLevel.City,
            ReferenceId = Guid.NewGuid(),
            AdjustmentPercentage = 0.010M,
            IsActive = true
        };
        var riskFactorConfigurationDto = new RiskFactorConfigurationDto
        {
            Id = riskFactorConfiguration.Id,
            Level = riskFactorConfiguration.Level,
            ReferenceId = riskFactorConfiguration.ReferenceId,
            AdjustmentPercentage = riskFactorConfiguration.AdjustmentPercentage,
            IsActive = riskFactorConfiguration.IsActive
        };
        _riskFactorRepository.Setup(r => r.GetAsync(riskFactorConfigurationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(riskFactorConfiguration);
        _mapper.Setup(m => m.Map<RiskFactorConfigurationDto>(riskFactorConfiguration)).Returns(riskFactorConfigurationDto);

        var result = await _riskFactorConfigurationService.GetRiskFactorConfigurationByIdAsync(riskFactorConfigurationId, CancellationToken.None);

        result.IsSuccess.Should().BeTrue("A valid Risk Factor Configuration Id should return a successful result");
        result.Errors.Should().BeEmpty("There should be no errors when passing an existing risk factor configuration id");
        result.Value.Should().NotBeNull("A RiskFactorConfigurationDto should be returned");
        result.Value.Should().BeEquivalentTo(riskFactorConfigurationDto);
        _mapper.Verify(m => m.Map<RiskFactorConfigurationDto>(riskFactorConfiguration), Times.Once);
        _riskFactorRepository.Verify(r => r.GetAsync(riskFactorConfigurationId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetRiskFactorConfigurationByIdAsync_GivenNonExistingId_ShouldReturnNotFound()
    {
        var riskFactorConfigurationId = Guid.NewGuid();
        RiskFactorConfiguration? riskFactorConfiguration = null;
        _riskFactorRepository.Setup(r => r.GetAsync(riskFactorConfigurationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(riskFactorConfiguration);

        var result = await _riskFactorConfigurationService.GetRiskFactorConfigurationByIdAsync(riskFactorConfigurationId, CancellationToken.None);

        result.IsFailed.Should().BeTrue("An invalid Risk Factor Configuration Id should return a fail result");
        result.Errors.Should().ContainSingle(e => e is NotFoundError
                                                  && e.Message == $"Risk Factor Configuration '{riskFactorConfigurationId}' not found.");
        _mapper.Verify(m => m.Map<RiskFactorConfigurationDto>(riskFactorConfiguration), Times.Never);
        _riskFactorRepository.Verify(r => r.GetAsync(riskFactorConfigurationId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateRiskFactorConfigurationAsync_GivenValidRequest_ShouldReturnConfigurationId()
    {
        var createRequest = new CreateRiskFactorConfigurationRequest()
        {
            Level = RiskFactorConfigurationLevel.City,
            ReferenceId = Guid.NewGuid(),
            AdjustmentPercentage = -0.015M,
            IsActive = true
        };
        var riskFactorConfiguration = new RiskFactorConfiguration
        {
            Id = Guid.NewGuid(),
            Level = createRequest.Level,
            ReferenceId = createRequest.ReferenceId,
            AdjustmentPercentage = createRequest.AdjustmentPercentage,
            IsActive = createRequest.IsActive
        };
        _requestValidator.Setup(v => v.ValidateAsync(createRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _riskFactorRepository.Setup(r =>
                r.ExistsBySameLevelAndReferenceIdAsync(createRequest.Level, createRequest.ReferenceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _mapper.Setup(m => m.Map<RiskFactorConfiguration>(createRequest)).Returns(riskFactorConfiguration);

        var result = await _riskFactorConfigurationService.CreateRiskFactorConfigurationAsync(createRequest, CancellationToken.None);

        result.IsSuccess.Should().BeTrue("A valid request should return a successful result");
        result.Errors.Should().BeEmpty("There should be no errors when passing a valid request");
        result.Value.Should().Be(riskFactorConfiguration.Id, "The Risk Factor Configuration Id should be returned");
        _requestValidator.Verify(v => v.ValidateAsync(createRequest, It.IsAny<CancellationToken>()), Times.Once);
        _riskFactorRepository.Verify(r => r.ExistsBySameLevelAndReferenceIdAsync(createRequest.Level, createRequest.ReferenceId, It.IsAny<CancellationToken>()), Times.Once);
        _mapper.Verify(m => m.Map<RiskFactorConfiguration>(createRequest), Times.Once);
        _riskFactorRepository.Verify(r => r.AddAsync(riskFactorConfiguration, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateRiskFactorConfigurationAsync_GivenInvalidRequest_ShouldReturnValidationError()
    {
        var createRequest = new CreateRiskFactorConfigurationRequest()
        {
            Level = RiskFactorConfigurationLevel.City,
            ReferenceId = Guid.NewGuid(),
            AdjustmentPercentage = -0.015M,
        };
        var failures = new[]
        {
            new ValidationFailure("IsActive", "IsActive is required and must be either true or false.")
            {
                ErrorCode = "IsActiveValidator"
            }
        };
        _requestValidator.Setup(v => v.ValidateAsync(createRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures));

        var result = await _riskFactorConfigurationService.CreateRiskFactorConfigurationAsync(createRequest, CancellationToken.None);

        result.IsFailed.Should().BeTrue("An invalid request should return a fail result");
        result.Errors.Should().ContainSingle(e => e is ValidationError);
        _requestValidator.Verify(v => v.ValidateAsync(createRequest, It.IsAny<CancellationToken>()), Times.Once);
        _riskFactorRepository.Verify(r => r.ExistsBySameLevelAndReferenceIdAsync(createRequest.Level, createRequest.ReferenceId, It.IsAny<CancellationToken>()), Times.Never);
        _mapper.Verify(m => m.Map<RiskFactorConfiguration>(createRequest), Times.Never);
        _riskFactorRepository.Verify(r => r.AddAsync(It.IsAny<RiskFactorConfiguration>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateRiskFactorConfigurationAsync_GivenExistingLevelAndReferenceId_ShouldReturnConflict()
    {
        var createRequest = new CreateRiskFactorConfigurationRequest()
        {
            Level = RiskFactorConfigurationLevel.City,
            ReferenceId = Guid.NewGuid(),
            AdjustmentPercentage = -0.015M,
            IsActive = true
        };
        _requestValidator.Setup(v => v.ValidateAsync(createRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _riskFactorRepository.Setup(r =>
                r.ExistsBySameLevelAndReferenceIdAsync(createRequest.Level, createRequest.ReferenceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _riskFactorConfigurationService.CreateRiskFactorConfigurationAsync(createRequest, CancellationToken.None);

        result.IsFailed.Should().BeTrue("A request with an already existing Level and ReferenceId combination should return a fail result");
        result.Errors.Should().ContainSingle(e => e is ConflictError);
        _requestValidator.Verify(v => v.ValidateAsync(createRequest, It.IsAny<CancellationToken>()), Times.Once);
        _riskFactorRepository.Verify(r => r.ExistsBySameLevelAndReferenceIdAsync(createRequest.Level, createRequest.ReferenceId, It.IsAny<CancellationToken>()), Times.Once);
        _mapper.Verify(m => m.Map<RiskFactorConfiguration>(createRequest), Times.Never);
        _riskFactorRepository.Verify(r => r.AddAsync(It.IsAny<RiskFactorConfiguration>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateRiskFactorConfigurationAsync_GivenExistingId_ShouldReturnRiskFactorConfiguration()
    {
        var riskFactorConfigurationId = Guid.NewGuid();
        var updateRequest = new UpdateRiskFactorConfigurationRequest()
        {
            IsActive = false
        };
        var riskFactorConfiguration = new RiskFactorConfiguration
        {
            Id = riskFactorConfigurationId,
            Level = RiskFactorConfigurationLevel.City,
            ReferenceId = Guid.NewGuid(),
            AdjustmentPercentage = 0.0125M,
            IsActive = true
        };
        _riskFactorRepository.Setup(r => r.GetAsync(riskFactorConfigurationId, It.IsAny<CancellationToken>())).ReturnsAsync(riskFactorConfiguration);
        _requestValidator.Setup(v => v.ValidateAsync(updateRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _riskFactorRepository.Setup(r =>
                r.ExistsBySameLevelAndReferenceIdAsync(updateRequest.Level ?? riskFactorConfiguration.Level,
                    updateRequest.ReferenceId ?? riskFactorConfiguration.ReferenceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _riskFactorConfigurationService.UpdateRiskFactorConfigurationAsync(riskFactorConfigurationId, updateRequest, CancellationToken.None);

        result.IsSuccess.Should().BeTrue("A valid request should return a successful result");
        result.Errors.Should().BeEmpty("There should be no errors when passing a valid request");
        result.Value.Should().Be(riskFactorConfiguration.Id, "The Risk Factor Configuration Id should be returned");
        _riskFactorRepository.Verify(r => r.GetAsync(riskFactorConfigurationId, It.IsAny<CancellationToken>()), Times.Once);
        _requestValidator.Verify(v => v.ValidateAsync(updateRequest, It.IsAny<CancellationToken>()), Times.Once);
        _riskFactorRepository.Verify(r => r.ExistsBySameLevelAndReferenceIdAsync(updateRequest.Level ?? riskFactorConfiguration.Level,
            updateRequest.ReferenceId ?? riskFactorConfiguration.ReferenceId, It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateRiskFactorConfigurationAsync_GivenNonExistingId_ShouldReturnNotFound()
    {
        var riskFactorConfigurationId = Guid.NewGuid();
        var updateRequest = new UpdateRiskFactorConfigurationRequest()
        {
            IsActive = false
        };
        RiskFactorConfiguration? riskFactorConfiguration = null;
        _riskFactorRepository.Setup(r => r.GetAsync(riskFactorConfigurationId, It.IsAny<CancellationToken>())).ReturnsAsync(riskFactorConfiguration);

        var result = await _riskFactorConfigurationService.UpdateRiskFactorConfigurationAsync(riskFactorConfigurationId, updateRequest, CancellationToken.None);

        result.IsFailed.Should().BeTrue("An invalid Risk Factor Configuration Id should return a fail result");
        result.Errors.Should().ContainSingle(e => e is NotFoundError
                                                  && e.Message == $"Risk Factor Configuration '{riskFactorConfigurationId}' not found.");
        _riskFactorRepository.Verify(r => r.GetAsync(riskFactorConfigurationId, It.IsAny<CancellationToken>()), Times.Once);
        _requestValidator.Verify(v => v.ValidateAsync(updateRequest, It.IsAny<CancellationToken>()), Times.Never);
        _riskFactorRepository.Verify(r => r.ExistsBySameLevelAndReferenceIdAsync(It.IsAny<RiskFactorConfigurationLevel>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateRiskFactorConfigurationAsync_GivenInvalidRequest_ShouldReturnValidationError()
    {
        var riskFactorConfigurationId = Guid.NewGuid();
        var updateRequest = new UpdateRiskFactorConfigurationRequest()
        {
            IsActive = false,
            Level = RiskFactorConfigurationLevel.City
        };
        var riskFactorConfiguration = new RiskFactorConfiguration
        {
            Id = riskFactorConfigurationId,
            Level = RiskFactorConfigurationLevel.City,
            ReferenceId = Guid.NewGuid(),
            AdjustmentPercentage = 0.0125M,
            IsActive = true
        };
        var failures = new[]
        {
            new ValidationFailure("ReferenceId", "ReferenceId is required for Geographic Configurations and must be a valid guid.")
            {
                ErrorCode = "ReferenceIdValidator"
            }
        };
        _riskFactorRepository.Setup(r => r.GetAsync(riskFactorConfigurationId, It.IsAny<CancellationToken>())).ReturnsAsync(riskFactorConfiguration);
        _requestValidator.Setup(v => v.ValidateAsync(updateRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures));

        var result = await _riskFactorConfigurationService.UpdateRiskFactorConfigurationAsync(riskFactorConfigurationId, updateRequest, CancellationToken.None);

        result.IsFailed.Should().BeTrue("An invalid request should return a fail result");
        result.Errors.Should().ContainSingle(e => e is ValidationError);
        _riskFactorRepository.Verify(r => r.GetAsync(riskFactorConfigurationId, It.IsAny<CancellationToken>()), Times.Once);
        _requestValidator.Verify(v => v.ValidateAsync(updateRequest, It.IsAny<CancellationToken>()), Times.Once);
        _riskFactorRepository.Verify(r => r.ExistsBySameLevelAndReferenceIdAsync(It.IsAny<RiskFactorConfigurationLevel>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }


    [Fact]
    public async Task UpdateRiskFactorConfigurationAsync_GivenExistingLevelAndReferenceId_ShouldReturnConflict()
    {
        var riskFactorConfigurationId = Guid.NewGuid();
        var updateRequest = new UpdateRiskFactorConfigurationRequest()
        {
            IsActive = false,
            ReferenceId = Guid.NewGuid()
        };
        var riskFactorConfiguration = new RiskFactorConfiguration
        {
            Id = riskFactorConfigurationId,
            Level = RiskFactorConfigurationLevel.City,
            ReferenceId = Guid.NewGuid(),
            AdjustmentPercentage = 0.0125M,
            IsActive = true
        };
        _riskFactorRepository.Setup(r => r.GetAsync(riskFactorConfigurationId, It.IsAny<CancellationToken>())).ReturnsAsync(riskFactorConfiguration);
        _requestValidator.Setup(v => v.ValidateAsync(updateRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _riskFactorRepository.Setup(r =>
                r.ExistsBySameLevelAndReferenceIdAsync(updateRequest.Level ?? riskFactorConfiguration.Level,
                    updateRequest.ReferenceId ?? riskFactorConfiguration.ReferenceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _riskFactorConfigurationService.UpdateRiskFactorConfigurationAsync(riskFactorConfigurationId, updateRequest, CancellationToken.None);

        result.IsFailed.Should().BeTrue("A request with an already existing Level and ReferenceId combination should return a fail result");
        result.Errors.Should().ContainSingle(e => e is ConflictError);
        _riskFactorRepository.Verify(r => r.GetAsync(riskFactorConfigurationId, It.IsAny<CancellationToken>()), Times.Once);
        _requestValidator.Verify(v => v.ValidateAsync(updateRequest, It.IsAny<CancellationToken>()), Times.Once);
        _riskFactorRepository.Verify(r => r.ExistsBySameLevelAndReferenceIdAsync(updateRequest.Level ?? riskFactorConfiguration.Level,
            updateRequest.ReferenceId ?? riskFactorConfiguration.ReferenceId, It.IsAny<CancellationToken>()), Times.Once);
    }
}
