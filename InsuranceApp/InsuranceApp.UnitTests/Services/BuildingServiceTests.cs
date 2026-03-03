using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using FluentValidation.Results;
using InsuranceApp.Application.Buildings;
using InsuranceApp.Application.Buildings.DTOs;
using InsuranceApp.Application.Common.Errors;
using InsuranceApp.Application.Common.Persistence;
using InsuranceApp.Application.Common.Repository;
using InsuranceApp.Application.Common.Validation;
using InsuranceApp.Domain.Entities;
using InsuranceApp.Domain.Enums;
using Microsoft.Extensions.Logging;
using Moq;

namespace InsuranceApp.UnitTests.Services;

public class BuildingServiceTests
{
    private readonly Mock<IBuildingRepository> _buildingRepository = new();
    private readonly Mock<IRiskIndicatorRepository> _riskIndicatorRepository = new();
    private readonly Mock<IRequestValidator> _requestValidator = new();
    private readonly Mock<IMapper> _mapper = new();
    private readonly Mock<ILogger<BuildingService>> _logger = new();
    private readonly IBuildingService _service;

    public BuildingServiceTests()
    {
        _service = new BuildingService(_buildingRepository.Object, _riskIndicatorRepository.Object,
            _requestValidator.Object, _mapper.Object, _logger.Object);
    }

    [Fact]
    public async Task GetBuildingByIdAsync_GivenExistingBuildingId_ShouldReturnBuilding()
    {
        var buildingId = Guid.NewGuid();
        var building = new Building
        {
            Id = buildingId,
            BuildingType = "Industrial",
            CityId = Guid.NewGuid(),
            ConstructionYear = 1970
        };
        var buildingDto = new BuildingDto()
        {
            Id = building.Id,
            BuildingType = building.BuildingType,
            ConstructionYear = building.ConstructionYear
        };

        _buildingRepository.Setup(r => r.GetBuildingWithIndicatorsById(buildingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(building);
        _mapper.Setup(m => m.Map<BuildingDto>(building)).Returns(buildingDto);

        var result = await _service.GetBuildingByIdAsync(buildingId, CancellationToken.None);

        result.IsSuccess.Should().BeTrue("A valid BuildingId should return a successful result");
        result.Errors.Should().BeEmpty("There should be no errors when passing an existing building id");
        result.Value.Should().NotBeNull("A BuildingDto should be returned");
        result.Value.Should().BeEquivalentTo(buildingDto);

        _mapper.Verify(m => m.Map<BuildingDto>(building), Times.Once);
        _buildingRepository.Verify(r => r.GetBuildingWithIndicatorsById(buildingId, It.IsAny<CancellationToken>()), Times.Once);

    }

    [Fact]
    public async Task GetBuildingByIdAsync_GivenNonExistingBuildingId_ShouldReturnNotFound()
    {
        var buildingId = Guid.NewGuid();
        Building? building = null;
        _buildingRepository.Setup(r => r.GetBuildingWithIndicatorsById(buildingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(building);

        var result = await _service.GetBuildingByIdAsync(buildingId, CancellationToken.None);

        result.IsFailed.Should().BeTrue("An invalid BuildingId should return a fail result");
        result.Errors.Should().ContainSingle(e => e is NotFoundError
                                                  && e.Message == $"Building '{buildingId}' not found.");

        _mapper.Verify(m => m.Map<BuildingDto>(building), Times.Never);
        _buildingRepository.Verify(r => r.GetBuildingWithIndicatorsById(buildingId, It.IsAny<CancellationToken>()), Times.Once);

    }

    [Fact]
    public async Task UpdateBuildingAsync_GivenValidRequest_ShouldReturnBuildingId()
    {
        var buildingId = Guid.NewGuid();
        var updateBuildingRequest = GetValidUpdateBuildingRequest();
        var building = GetValidBuilding();
        var riskIndicators = new List<RiskIndicator>
        {
            new() { Id = 1, Name = "Some risk" },
            new() { Id = 2, Name = "Some risk 2" }
        };
        _buildingRepository.Setup(r => r.GetAsync(buildingId, It.IsAny<CancellationToken>())).ReturnsAsync(building);
        _requestValidator.Setup(v => v.ValidateAsync(updateBuildingRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _riskIndicatorRepository.Setup(r => r.FindAsync(It.IsAny<Expression<Func<RiskIndicator, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(riskIndicators);

        var result = await _service.UpdateBuildingAsync(buildingId, updateBuildingRequest, CancellationToken.None);

        result.IsSuccess.Should().BeTrue("A valid request should return a successful result");
        result.Errors.Should().BeEmpty("There should be no errors when passing a valid request");
        result.Value.Should().Be(building.Id, "The building's Guid should be returned");

        _buildingRepository.Verify(r => r.GetAsync(buildingId, It.IsAny<CancellationToken>()), Times.Once);
        _requestValidator.Verify(v => v.ValidateAsync(updateBuildingRequest, It.IsAny<CancellationToken>()), Times.Once);
        _riskIndicatorRepository.Verify(r =>
            r.FindAsync(It.IsAny<Expression<Func<RiskIndicator, bool>>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateBuildingAsync_GivenNoRiskIndicators_ShouldReturnBuildingId()
    {
        var buildingId = Guid.NewGuid();
        var updateBuildingRequest = GetValidUpdateBuildingRequest();
        updateBuildingRequest.RiskIndicatorIds = new List<int>();
        var building = GetValidBuilding();
        _buildingRepository.Setup(r => r.GetAsync(buildingId, It.IsAny<CancellationToken>())).ReturnsAsync(building);
        _requestValidator.Setup(v => v.ValidateAsync(updateBuildingRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        var result = await _service.UpdateBuildingAsync(buildingId, updateBuildingRequest, CancellationToken.None);

        result.IsSuccess.Should().BeTrue("A valid request should return a successful result");
        result.Errors.Should().BeEmpty("There should be no errors when passing a valid request");
        result.Value.Should().Be(building.Id, "The building's Guid should be returned");

        _buildingRepository.Verify(r => r.GetAsync(buildingId, It.IsAny<CancellationToken>()), Times.Once);
        _requestValidator.Verify(v => v.ValidateAsync(updateBuildingRequest, It.IsAny<CancellationToken>()), Times.Once);
        _riskIndicatorRepository.Verify(r =>
            r.FindAsync(It.IsAny<Expression<Func<RiskIndicator, bool>>>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateBuildingAsync_GivenNonExistingBuildingId_ShouldReturnNotFound()
    {
        var buildingId = Guid.NewGuid();
        var updateBuildingRequest = GetValidUpdateBuildingRequest();
        Building? building = null;
        _buildingRepository.Setup(r => r.GetAsync(buildingId, It.IsAny<CancellationToken>())).ReturnsAsync(building);

        var result = await _service.UpdateBuildingAsync(buildingId, updateBuildingRequest, CancellationToken.None);

        result.IsFailed.Should().BeTrue("An invalid BuildingId should return a fail result");
        result.Errors.Should().ContainSingle(e => e is NotFoundError
                                                  && e.Message == $"Building '{buildingId}' not found.");

        _buildingRepository.Verify(r => r.GetAsync(buildingId, It.IsAny<CancellationToken>()), Times.Once);
        _requestValidator.Verify(v => v.ValidateAsync(updateBuildingRequest, It.IsAny<CancellationToken>()), Times.Never);
        _riskIndicatorRepository.Verify(r =>
            r.FindAsync(It.IsAny<Expression<Func<RiskIndicator, bool>>>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateBuildingAsync_GivenInvalidRequest_ShouldReturnValidationError()
    {
        var buildingId = Guid.NewGuid();
        var updateBuildingRequest = GetValidUpdateBuildingRequest();
        updateBuildingRequest.InsuredValue = -1250M;
        var building = GetValidBuilding();
        var failures = new[]
        {
            new ValidationFailure("InsuredValue", "InsuredValue must be greater than 0.")
            {
                ErrorCode = "GreaterThanValidator"
            }
        };
        _buildingRepository.Setup(r => r.GetAsync(buildingId, It.IsAny<CancellationToken>())).ReturnsAsync(building);
        _requestValidator.Setup(v => v.ValidateAsync(updateBuildingRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures));

        var result = await _service.UpdateBuildingAsync(buildingId, updateBuildingRequest, CancellationToken.None);

        result.IsFailed.Should().BeTrue("An invalid BuildingId should return a fail result");
        result.Errors.Should().ContainSingle(e => e is ValidationError);

        _buildingRepository.Verify(r => r.GetAsync(buildingId, It.IsAny<CancellationToken>()), Times.Once);
        _requestValidator.Verify(v => v.ValidateAsync(updateBuildingRequest, It.IsAny<CancellationToken>()), Times.Once);
        _riskIndicatorRepository.Verify(r =>
            r.FindAsync(It.IsAny<Expression<Func<RiskIndicator, bool>>>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    private Building GetValidBuilding() => new Building()
    {
        Id = Guid.NewGuid(),
        Address = "Some address",
        BuildingType = "Office",
        CityId = Guid.NewGuid(),
        ConstructionYear = 1970,
        InsuredValue = 2000000,
        NumberOfFloors = 2,
        SurfaceArea = 123.52M
    };

    private UpdateBuildingRequest GetValidUpdateBuildingRequest() => new UpdateBuildingRequest()
    {
        Address = "Some address",
        BuildingType = BuildingType.Industrial,
        CityId = Guid.NewGuid(),
        ConstructionYear = 1970,
        InsuredValue = 2000000,
        NumberOfFloors = 2,
        RiskIndicatorIds = new List<int>() { 1, 3 },
        SurfaceArea = 123.52M
    };
}
