using InsuranceApp.Domain.Entities;
using InsuranceApp.Domain.Enums;
using InsuranceApp.Domain.Models;

namespace InsuranceApp.Domain.Pricing.ConfigurationStrategies.Fees;

public class RiskAdjustmentFeeStrategy : IFeeConfigurationStrategy
{
    public FeeConfigurationType Type => FeeConfigurationType.RiskAdjustment;
    public IEnumerable<Adjustment> GetAdjustments(Building building, IEnumerable<FeeConfiguration> configurations)
    {
        var buildingRiskIndicators = building.RiskIndicators.Select(r => r.Name.ToLower());
        var applicableAdjustments = configurations
            .Where(c => buildingRiskIndicators.Contains(c.Name.ToLower().Split("adjustment")[0].TrimEnd()))
            .Select(c => new Adjustment() { Type = AdjustmentTypeEnum.FeeConfiguration, SubType = Type.ToString(), Percentage = c.Percentage });
        return applicableAdjustments;
    }
}