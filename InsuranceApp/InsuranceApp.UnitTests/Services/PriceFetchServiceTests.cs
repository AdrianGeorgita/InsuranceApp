using FluentAssertions;
using InsuranceApp.Application.Common.Repository;
using InsuranceApp.Application.Policies.DTOs;
using InsuranceApp.Application.Policies.Pricing;
using InsuranceApp.Domain.Entities;
using InsuranceApp.Domain.Enums;
using Moq;

namespace InsuranceApp.UnitTests.Services;

public class PriceFetchServiceTests
{
    private readonly Mock<IRiskFactorConfigurationRepository> _riskFactorConfigurationRepository = new();
    private readonly Mock<IFeeConfigurationRepository> _feeConfigurationRepository = new();
    private readonly Mock<IBuildingRepository> _buildingRepository = new();
    private readonly Mock<IBrokerRepository> _brokerRepository = new();
    private readonly IPriceFetchService _priceFetchService;

    public PriceFetchServiceTests()
    {
        _priceFetchService = new PriceFetchService(_riskFactorConfigurationRepository.Object, _feeConfigurationRepository.Object,
            _buildingRepository.Object, _brokerRepository.Object);
    }

    [Fact]
    public async Task FetchPricingContextAsync_GivenPolicyWithActiveConfigurations_ShouldPricingContextWithConfigurations()
    {
        var startDate = new DateTime(2026, 04, 01, 0, 0, 0, DateTimeKind.Utc);
        var building = GetValidBuilding();
        var broker = GetValidBroker();
        _buildingRepository.Setup(r => r.GetBuildingWithIndicatorsById(building.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(building);
        var riskConfigurations = GetListOfRiskFactorConfigurations();
        _riskFactorConfigurationRepository.Setup(r => r.GetActiveConfigurationsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(riskConfigurations);
        var feeConfigurations = GetListOfFeeConfigurations();
        _feeConfigurationRepository.Setup(r => r.GetActiveConfigurationsAtDateTimeAsync(startDate,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(feeConfigurations);
        _brokerRepository.Setup(r => r.GetAsync(broker.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(broker);

        var result =
            await _priceFetchService.FetchPricingContextAsync(building.Id, startDate, broker.Id,
                CancellationToken.None);

        result.Should().NotBeNull("A context should always be returned.");
        result.Should().BeEquivalentTo(new PricingContextDto()
        {
            Broker = broker,
            Building = building,
            FeeConfigurations = feeConfigurations,
            RiskFactorConfigurations = riskConfigurations
        });
        _buildingRepository.Verify(r => r.GetBuildingWithIndicatorsById(building.Id, It.IsAny<CancellationToken>()), Times.Once);
        _riskFactorConfigurationRepository.Verify(r => r.GetActiveConfigurationsAsync(It.IsAny<CancellationToken>()), Times.Once);
        _feeConfigurationRepository.Verify(r => r.GetActiveConfigurationsAtDateTimeAsync(startDate, It.IsAny<CancellationToken>()), Times.Once);
        _brokerRepository.Verify(r => r.GetAsync(broker.Id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task FetchPricingContextAsync_GivenPolicyWithNoActiveConfigurations_ShouldPricingContextWithConfigurations()
    {
        var startDate = new DateTime(2026, 04, 01, 0, 0, 0, DateTimeKind.Utc);
        var building = GetValidBuilding();
        var broker = GetValidBroker();
        _buildingRepository.Setup(r => r.GetBuildingWithIndicatorsById(building.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(building);
        _riskFactorConfigurationRepository.Setup(r => r.GetActiveConfigurationsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<RiskFactorConfiguration>());
        _feeConfigurationRepository.Setup(r => r.GetActiveConfigurationsAtDateTimeAsync(startDate,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<FeeConfiguration>());
        _brokerRepository.Setup(r => r.GetAsync(broker.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(broker);

        var result =
            await _priceFetchService.FetchPricingContextAsync(building.Id, startDate, broker.Id,
                CancellationToken.None);

        result.Should().NotBeNull("A context should always be returned.");
        result.Should().BeEquivalentTo(new PricingContextDto()
        {
            Broker = broker,
            Building = building,
            FeeConfigurations = new List<FeeConfiguration>(),
            RiskFactorConfigurations = new List<RiskFactorConfiguration>()
        });
        _buildingRepository.Verify(r => r.GetBuildingWithIndicatorsById(building.Id, It.IsAny<CancellationToken>()), Times.Once);
        _riskFactorConfigurationRepository.Verify(r => r.GetActiveConfigurationsAsync(It.IsAny<CancellationToken>()), Times.Once);
        _feeConfigurationRepository.Verify(r => r.GetActiveConfigurationsAtDateTimeAsync(startDate, It.IsAny<CancellationToken>()), Times.Once);
        _brokerRepository.Verify(r => r.GetAsync(broker.Id, It.IsAny<CancellationToken>()), Times.Once);
    }

    private Building GetValidBuilding() => new Building()
    {
        Id = Guid.NewGuid(),
        BuildingType = "Industrial",
        CityId = Guid.NewGuid()
    };
    private Broker GetValidBroker() => new Broker()
    {
        Id = Guid.NewGuid(),
        CommissionPercentage = 0.0020M,
        Status = BrokerStatus.Active
    };
    private List<RiskFactorConfiguration> GetListOfRiskFactorConfigurations() =>
    [
        new() { Level = RiskFactorConfigurationLevel.BuildingType, BuildingType = BuildingType.Industrial }
    ];

    private List<FeeConfiguration> GetListOfFeeConfigurations() =>
    [
        new() { Type = FeeConfigurationType.AdminFee },
        new() { Type = FeeConfigurationType.BrokerCommission }
    ];
}
