using InsuranceApp.Domain.Entities;
using InsuranceApp.Domain.Enums;
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

    public IEnumerable<decimal> GetAllApplicableAdjustments(Building building, Broker broker,
        IEnumerable<RiskFactorConfiguration> riskFactorConfigurations, IEnumerable<FeeConfiguration> feeConfigurations)
    {
        var adjustments = new List<decimal>();
        adjustments.AddRange(GetApplicableRiskFactorConfigurations(riskFactorConfigurations, building));
        adjustments.AddRange(GetApplicableFeeConfigurations(feeConfigurations, building));
        adjustments.Add(GetBrokerCommission(broker));
        return adjustments;
    }

    private List<decimal> GetApplicableFeeConfigurations(IEnumerable<FeeConfiguration> configurations, Building building)
    {
        var adjustments = new List<decimal>();
        foreach (var group in configurations.GroupBy(c => c.Type))
        {
            if (!_feeConfigurationStrategies.TryGetValue(group.Key, out var strategy))
                continue;

            adjustments.AddRange(strategy.GetAdjustments(building, group));
        }
        return adjustments;
    }

    private List<decimal> GetApplicableRiskFactorConfigurations(IEnumerable<RiskFactorConfiguration> configurations, Building building)
    {
        var adjustments = new List<decimal>();
        foreach (var group in configurations.GroupBy(c => c.Level))
        {
            if (!_riskFactorConfigurationStrategies.TryGetValue(group.Key, out var strategy))
                continue;

            adjustments.AddRange(strategy.GetAdjustments(building, group));
        }

        return adjustments;
    }

    private static decimal GetBrokerCommission(Broker broker) => broker?.CommissionPercentage ?? 0M;
}