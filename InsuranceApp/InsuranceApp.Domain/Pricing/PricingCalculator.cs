using InsuranceApp.Domain.Entities;
using InsuranceApp.Domain.Enums;
using InsuranceApp.Domain.Models;
using InsuranceApp.Domain.Pricing.ConfigurationStrategies.Fees;
using InsuranceApp.Domain.Pricing.ConfigurationStrategies.RiskFactors;

namespace InsuranceApp.Domain.Pricing;

public class PricingCalculator(IEnumerable<IRiskFactorConfigurationStrategy> riskFactorConfigurationStrategies,
    IEnumerable<IFeeConfigurationStrategy> feeConfigurationStrategies) : IPricingCalculator
{
    private readonly Dictionary<RiskFactorConfigurationLevel, IRiskFactorConfigurationStrategy>
        _riskFactorConfigurationStrategies =
            riskFactorConfigurationStrategies.ToDictionary(s => s.Level);

    private readonly Dictionary<FeeConfigurationType, IFeeConfigurationStrategy>
        _feeConfigurationStrategies =
            feeConfigurationStrategies.ToDictionary(s => s.Type);

    public IEnumerable<Adjustment> GetAllApplicableAdjustments(Building building, Broker broker,
        IEnumerable<RiskFactorConfiguration> riskFactorConfigurations, IEnumerable<FeeConfiguration> feeConfigurations)
    {
        var adjustments = new List<Adjustment>();
        adjustments.AddRange(GetApplicableRiskFactorConfigurations(riskFactorConfigurations, building));
        adjustments.AddRange(GetApplicableFeeConfigurations(feeConfigurations, building));
        adjustments.Add(GetBrokerCommissionAdjustment(broker));
        return adjustments;
    }

    private List<Adjustment> GetApplicableFeeConfigurations(IEnumerable<FeeConfiguration> configurations, Building building)
    {
        var adjustments = new List<Adjustment>();
        foreach (var group in configurations.GroupBy(c => c.Type))
        {
            if (!_feeConfigurationStrategies.TryGetValue(group.Key, out var strategy))
                continue;

            adjustments.AddRange(strategy.GetAdjustments(building, group));
        }
        return adjustments;
    }

    private List<Adjustment> GetApplicableRiskFactorConfigurations(IEnumerable<RiskFactorConfiguration> configurations, Building building)
    {
        var adjustments = new List<Adjustment>();
        foreach (var group in configurations.GroupBy(c => c.Level))
        {
            if (!_riskFactorConfigurationStrategies.TryGetValue(group.Key, out var strategy))
                continue;

            adjustments.AddRange(strategy.GetAdjustments(building, group));
        }

        return adjustments;
    }

    private static Adjustment GetBrokerCommissionAdjustment(Broker broker) => new Adjustment() {Type = AdjustmentTypeEnum.BrokerCommission, 
        Percentage = broker?.CommissionPercentage ?? 0M};
}