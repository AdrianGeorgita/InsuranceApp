using FluentAssertions;
using InsuranceApp.Domain.Entities;
using InsuranceApp.Domain.Enums;
using InsuranceApp.Domain.Models;
using InsuranceApp.Domain.Pricing;
using InsuranceApp.Domain.Pricing.ConfigurationStrategies.Fees;
using InsuranceApp.Domain.Pricing.ConfigurationStrategies.RiskFactors;
using Moq;

namespace InsuranceApp.UnitTests.Domain;

public class PricingCalculatorTests
{
    [Fact]
    public void GetAllApplicableAdjustments_GivenPolicyWithActiveConfigurations_ShouldReturnListOfAdjustments()
    {
        var building = GetValidBuilding();
        var riskConfigurations = GetListOfRiskFactorConfigurations();
        var feeConfigurations = GetListOfFeeConfigurations();
        var broker = GetValidBroker();
        var riskBuildingTypeStrategy = new Mock<IRiskFactorConfigurationStrategy>();
        riskBuildingTypeStrategy.SetupGet(s => s.Level).Returns(RiskFactorConfigurationLevel.BuildingType);
        riskBuildingTypeStrategy
            .Setup(s => s.GetAdjustments(
                building,
                It.Is<IEnumerable<RiskFactorConfiguration>>(g => g.All(x => x.Level == RiskFactorConfigurationLevel.BuildingType))))
            .Returns([new Adjustment(){Type = AdjustmentTypeEnum.RiskFactorConfiguration, SubType = "BuildingType", Percentage = 0.1000M}]);
        var feeAdminStrategy = new Mock<IFeeConfigurationStrategy>();
        feeAdminStrategy.SetupGet(s => s.Type).Returns(FeeConfigurationType.AdminFee);
        feeAdminStrategy
            .Setup(s => s.GetAdjustments(
                building,
                It.Is<IEnumerable<FeeConfiguration>>(g => g.All(x => x.Type == FeeConfigurationType.AdminFee))))
            .Returns([new Adjustment() { Type = AdjustmentTypeEnum.FeeConfiguration, SubType = "AdminFee", Percentage = 0.0010M }]);
        var brokerCommissionStrategy = new Mock<IFeeConfigurationStrategy>();
        brokerCommissionStrategy.SetupGet(s => s.Type).Returns(FeeConfigurationType.BrokerCommission);
        brokerCommissionStrategy
            .Setup(s => s.GetAdjustments(
                building,
                It.Is<IEnumerable<FeeConfiguration>>(g => g.All(x => x.Type == FeeConfigurationType.BrokerCommission))))
            .Returns([new Adjustment() { Type = AdjustmentTypeEnum.FeeConfiguration, SubType = "BrokerCommission", Percentage = 0.0050M }]);
        var pricingCalculator = new PricingCalculator(
            [riskBuildingTypeStrategy.Object],
            [feeAdminStrategy.Object, brokerCommissionStrategy.Object]);

        var result = pricingCalculator.GetAllApplicableAdjustments(building, broker, riskConfigurations, feeConfigurations).ToList();

        result.Should().HaveCount(4, "The configurations should return 4 adjustments");
        result.Should().BeEquivalentTo([
            new Adjustment() { Type = AdjustmentTypeEnum.RiskFactorConfiguration, SubType = "BuildingType", Percentage = 0.1000M },
            new Adjustment() { Type = AdjustmentTypeEnum.FeeConfiguration, SubType = "AdminFee", Percentage = 0.0010M },
            new Adjustment() { Type = AdjustmentTypeEnum.FeeConfiguration, SubType = "BrokerCommission", Percentage = 0.0050M },
            new Adjustment() { Type = AdjustmentTypeEnum.BrokerCommission, Percentage = 0.0020M}
        ]);
        riskBuildingTypeStrategy.Verify(s => s.GetAdjustments(building, It.IsAny<IEnumerable<RiskFactorConfiguration>>()), Times.Once);
        feeAdminStrategy.Verify(s => s.GetAdjustments(building, It.IsAny<IEnumerable<FeeConfiguration>>()), Times.Once);
        brokerCommissionStrategy.Verify(s => s.GetAdjustments(building, It.IsAny<IEnumerable<FeeConfiguration>>()), Times.Once);
    }

    [Fact]
    public void GetAllApplicableAdjustments_GivenPolicyWithNoActiveConfigurations_ShouldReturnEmptyListOfAdjustments()
    {
        var building = GetValidBuilding();
        var broker = GetValidBroker();
        broker.CommissionPercentage = 0.0M;
        var riskConfigurations = GetListOfRiskFactorConfigurations();
        riskConfigurations.Clear();
        var feeConfigurations = GetListOfFeeConfigurations();
        feeConfigurations.Clear();
        var riskBuildingTypeStrategy = new Mock<IRiskFactorConfigurationStrategy>();
        riskBuildingTypeStrategy.SetupGet(s => s.Level).Returns(RiskFactorConfigurationLevel.BuildingType);
        riskBuildingTypeStrategy
            .Setup(s => s.GetAdjustments(
                building,
                It.Is<IEnumerable<RiskFactorConfiguration>>(g => g.All(x => x.Level == RiskFactorConfigurationLevel.BuildingType))))
            .Returns([new Adjustment() { Type = AdjustmentTypeEnum.RiskFactorConfiguration, SubType = "BuildingType", Percentage = 0.1000M }]);
        var feeAdminStrategy = new Mock<IFeeConfigurationStrategy>();
        feeAdminStrategy.SetupGet(s => s.Type).Returns(FeeConfigurationType.AdminFee);
        feeAdminStrategy
            .Setup(s => s.GetAdjustments(
                building,
                It.Is<IEnumerable<FeeConfiguration>>(g => g.All(x => x.Type == FeeConfigurationType.AdminFee))))
            .Returns([new Adjustment() { Type = AdjustmentTypeEnum.FeeConfiguration, SubType = "AdminFee", Percentage = 0.0010M }]);
        var brokerCommissionStrategy = new Mock<IFeeConfigurationStrategy>();
        brokerCommissionStrategy.SetupGet(s => s.Type).Returns(FeeConfigurationType.BrokerCommission);
        brokerCommissionStrategy
            .Setup(s => s.GetAdjustments(
                building,
                It.Is<IEnumerable<FeeConfiguration>>(g => g.All(x => x.Type == FeeConfigurationType.BrokerCommission))))
            .Returns([new Adjustment() { Type = AdjustmentTypeEnum.FeeConfiguration, SubType = "BrokerCommission", Percentage = 0.0050M }]);
        var pricingCalculator = new PricingCalculator(
            [riskBuildingTypeStrategy.Object],
            [feeAdminStrategy.Object, brokerCommissionStrategy.Object]);

        var result = pricingCalculator.GetAllApplicableAdjustments(building, broker, riskConfigurations, feeConfigurations).ToList();

        result.Should().HaveCount(1, "The configurations should return 1 adjustment");
        result.Should().BeEquivalentTo(new List<Adjustment>() { new (){Type = AdjustmentTypeEnum.BrokerCommission, Percentage = 0M} });
    }

    private Building GetValidBuilding() => new Building()
    {
        Id = Guid.NewGuid(),
        BuildingType = "Industrial"
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

    private Broker GetValidBroker() => new Broker()
    {
        Id = Guid.NewGuid(),
        CommissionPercentage = 0.0020M,
        Status = BrokerStatus.Active
    };


}
