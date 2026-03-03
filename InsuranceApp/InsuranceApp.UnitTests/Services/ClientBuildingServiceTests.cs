using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using FluentValidation.Results;
using InsuranceApp.Application.Buildings.DTOs;
using InsuranceApp.Application.Clients.Buildings;
using InsuranceApp.Application.Common.Errors;
using InsuranceApp.Application.Common.Pagination;
using InsuranceApp.Application.Common.Persistence;
using InsuranceApp.Application.Common.Repository;
using InsuranceApp.Application.Common.Validation;
using InsuranceApp.Domain.Entities;
using InsuranceApp.Domain.Enums;
using Microsoft.Extensions.Logging;
using Moq;

namespace InsuranceApp.UnitTests.Services;

public class ClientBuildingServiceTests
{
    private readonly Mock<IClientRepository> _clientRepository = new();
    private readonly Mock<IBuildingRepository> _buildingRepository = new();
    private readonly Mock<IRiskIndicatorRepository> _riskIndicatorRepository = new();
    private readonly Mock<IRequestValidator> _requestValidator = new();
    private readonly Mock<IMapper> _mapper = new();
    private readonly Mock<ILogger<ClientBuildingService>> _logger = new();
    private readonly IClientBuildingService _service;

    public ClientBuildingServiceTests()
    {
        _service = new ClientBuildingService(_clientRepository.Object, _buildingRepository.Object,
            _riskIndicatorRepository.Object, _requestValidator.Object, _mapper.Object, _logger.Object);
    }

    [Fact]
    public async Task ListAllClientBuildingsAsync_GivenExistingClientId_ShouldReturnPagedListOfBuildings()
    {
        var clientId = Guid.NewGuid();
        var pageRequest = new PageRequest { PageNumber = 1, PageSize = 10 };

        var client = new Client() { Id = clientId };

        var pagedBuildings = new PagedResult<BuildingDto>
        {
            Items = new List<BuildingDto>
            {
                new() { Id = Guid.NewGuid(), OwnerId = clientId },
                new() { Id = Guid.NewGuid(), OwnerId = clientId },
                new() { Id = Guid.NewGuid(), OwnerId = clientId },
            },
            TotalCount = 3,
            PageNumber = 1,
            PageSize = 10
        };
        _clientRepository.Setup(r => r.GetAsync(clientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(client);
        _buildingRepository.Setup(r => r.GetAllClientBuildings(clientId, pageRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedBuildings);
        var result = await _service.ListAllClientBuildingsAsync(clientId, pageRequest, CancellationToken.None);

        result.IsSuccess.Should().BeTrue("A paged list of buildings should be returned in case of existing client");
        result.Errors.Should().BeEmpty("There should be no errors when fetching a paged list of buildings");
        result.Value.Should().NotBeNull("A PagedResult should be returned");
        result.Value.Should().BeEquivalentTo(pagedBuildings);

        _clientRepository.Verify(r => r.GetAsync(clientId, It.IsAny<CancellationToken>()), Times.Once);
        _buildingRepository.Verify(r => r.GetAllClientBuildings(clientId, pageRequest, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ListAllClientBuildingsAsync_GivenNonExistingClientId_ShouldReturnNotFound()
    {
        var clientId = Guid.NewGuid();
        var pageRequest = new PageRequest { PageNumber = 1, PageSize = 10 };
        Client? client = null;
        _clientRepository.Setup(r => r.GetAsync(clientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(client);

        var result = await _service.ListAllClientBuildingsAsync(clientId, pageRequest, CancellationToken.None);

        result.IsFailed.Should().BeTrue("An invalid ClientId should return a fail result");
        result.Errors.Should().ContainSingle(e => e is NotFoundError
                                                  && e.Message == $"Client '{clientId}' not found.");

        _clientRepository.Verify(r => r.GetAsync(clientId, It.IsAny<CancellationToken>()), Times.Once);
        _buildingRepository.Verify(r => r.GetAllClientBuildings(clientId, pageRequest, It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateClientBuildingAsync_GivenValidRequest_ShouldReturnBuildingId()
    {
        var clientId = Guid.NewGuid();
        var createBuildingRequest = GetValidCreateBuildingRequest();
        var client = new Client { Id = clientId };
        var building = new Building
        {
            Id = Guid.NewGuid(),
            Address = createBuildingRequest.Address,
            BuildingType = createBuildingRequest.BuildingType.ToString(),
            CityId = createBuildingRequest.CityId,
            ConstructionYear = createBuildingRequest.ConstructionYear,
            InsuredValue = createBuildingRequest.InsuredValue,
            NumberOfFloors = createBuildingRequest.NumberOfFloors,
            SurfaceArea = createBuildingRequest.SurfaceArea
        };
        var riskIndicators = new List<RiskIndicator>
        {
            new() { Id = 1, Name = "Some risk" },
            new() { Id = 2, Name = "Some risk 2" }
        };
        _clientRepository.Setup(r => r.GetAsync(clientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(client);
        _requestValidator.Setup(v => v.ValidateAsync(createBuildingRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _mapper.Setup(m => m.Map<Building>(createBuildingRequest)).Returns(building);
        _riskIndicatorRepository.Setup(r => r.FindAsync(It.IsAny<Expression<Func<RiskIndicator, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(riskIndicators);

        var result = await _service.CreateClientBuildingAsync(clientId, createBuildingRequest, CancellationToken.None);

        result.IsSuccess.Should().BeTrue("A valid request should return a successful result");
        result.Errors.Should().BeEmpty("There should be no errors when passing a valid request");
        result.Value.Should().Be(building.Id, "The building's Guid should be returned");

        _clientRepository.Verify(r => r.GetAsync(clientId, It.IsAny<CancellationToken>()), Times.Once);
        _requestValidator.Verify(v => v.ValidateAsync(createBuildingRequest, It.IsAny<CancellationToken>()), Times.Once);
        _mapper.Verify(m => m.Map<Building>(createBuildingRequest), Times.Once);
        _riskIndicatorRepository.Verify(r =>
            r.FindAsync(It.IsAny<Expression<Func<RiskIndicator, bool>>>(), It.IsAny<CancellationToken>()), Times.Once);
        _buildingRepository.Verify(r => r.AddAsync(building, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateClientBuildingAsync_GivenNonExistingClientId_ShouldReturnNotFound()
    {
        var clientId = Guid.NewGuid();
        var createBuildingRequest = GetValidCreateBuildingRequest();
        Client? client = null;
        _clientRepository.Setup(r => r.GetAsync(clientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(client);

        var result = await _service.CreateClientBuildingAsync(clientId, createBuildingRequest, CancellationToken.None);

        result.IsFailed.Should().BeTrue("An invalid ClientId should return a fail result");
        result.Errors.Should().ContainSingle(e => e is NotFoundError
                                                  && e.Message == $"Client '{clientId}' not found.");

        _clientRepository.Verify(r => r.GetAsync(clientId, It.IsAny<CancellationToken>()), Times.Once);
        _requestValidator.Verify(v => v.ValidateAsync(createBuildingRequest, It.IsAny<CancellationToken>()), Times.Never);
        _mapper.Verify(m => m.Map<Building>(createBuildingRequest), Times.Never);
        _riskIndicatorRepository.Verify(r =>
            r.FindAsync(It.IsAny<Expression<Func<RiskIndicator, bool>>>(), It.IsAny<CancellationToken>()), Times.Never);
        _buildingRepository.Verify(r => r.AddAsync(It.IsAny<Building>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateClientBuildingAsync_GivenInvalidRequest_ShouldReturnValidationError()
    {
        var clientId = Guid.NewGuid();
        var createBuildingRequest = GetValidCreateBuildingRequest();
        var client = new Client { Id = clientId };
        var failures = new[]
        {
            new ValidationFailure("NumberOfFloors", "NumbersOfFloors must be provided.")
            {
                ErrorCode = "NotFound"
            }
        };
        _clientRepository.Setup(r => r.GetAsync(clientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(client);
        _requestValidator.Setup(v => v.ValidateAsync(createBuildingRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures));

        var result = await _service.CreateClientBuildingAsync(clientId, createBuildingRequest, CancellationToken.None);

        result.IsFailed.Should().BeTrue("An invalid request should return a fail result");
        result.Errors.Should().ContainSingle(e => e is ValidationError);

        _clientRepository.Verify(r => r.GetAsync(clientId, It.IsAny<CancellationToken>()), Times.Once);
        _requestValidator.Verify(v => v.ValidateAsync(createBuildingRequest, It.IsAny<CancellationToken>()), Times.Once);
        _mapper.Verify(m => m.Map<Building>(createBuildingRequest), Times.Never);
        _riskIndicatorRepository.Verify(r =>
            r.FindAsync(It.IsAny<Expression<Func<RiskIndicator, bool>>>(), It.IsAny<CancellationToken>()), Times.Never);
        _buildingRepository.Verify(r => r.AddAsync(It.IsAny<Building>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    private CreateBuildingRequest GetValidCreateBuildingRequest() => new CreateBuildingRequest()
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
