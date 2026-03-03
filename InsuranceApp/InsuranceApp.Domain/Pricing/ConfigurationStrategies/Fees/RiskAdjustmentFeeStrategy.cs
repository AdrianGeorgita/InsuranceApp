using InsuranceApp.Domain.Entities;
using InsuranceApp.Domain.Enums;

namespace InsuranceApp.Domain.Pricing.ConfigurationStrategies.Fees;

public class RiskAdjustmentFeeStrategy : IFeeConfigurationStrategy
{
    public FeeConfigurationType Type => FeeConfigurationType.RiskAdjustment;
    public IEnumerable<decimal> GetAdjustments(Building building, IEnumerable<FeeConfiguration> configurations)
    {
        var buildingRiskIndicators = building.RiskIndicators.Select(r => r.Name.ToLower());
        var applicableAdjustments = configurations
            .Where(c => buildingRiskIndicators.Contains(c.Name.ToLower().Split("adjustment")[0].TrimEnd()))
            .Select(c => c.Percentage);
        return applicableAdjustments;
    }
}