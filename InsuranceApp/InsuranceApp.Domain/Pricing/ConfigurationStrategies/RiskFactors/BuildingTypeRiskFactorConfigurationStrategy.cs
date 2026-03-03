using InsuranceApp.Domain.Entities;
using InsuranceApp.Domain.Enums;

namespace InsuranceApp.Domain.Pricing.ConfigurationStrategies.RiskFactors;

public class BuildingTypeRiskFactorConfigurationStrategy : IRiskFactorConfigurationStrategy
{
    public RiskFactorConfigurationLevel Level => RiskFactorConfigurationLevel.BuildingType;
    public IEnumerable<decimal> GetAdjustments(Building building, IEnumerable<RiskFactorConfiguration> configurations)
    {
        var applicableAdjustments = configurations
            .Where(c => c.BuildingType.ToString() == building.BuildingType)
            .Select(c => c.AdjustmentPercentage);
        return applicableAdjustments;
    }
}