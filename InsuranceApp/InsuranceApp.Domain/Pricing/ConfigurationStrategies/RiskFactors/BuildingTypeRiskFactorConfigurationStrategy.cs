using InsuranceApp.Domain.Entities;
using InsuranceApp.Domain.Enums;
using InsuranceApp.Domain.Models;

namespace InsuranceApp.Domain.Pricing.ConfigurationStrategies.RiskFactors;

public class BuildingTypeRiskFactorConfigurationStrategy : IRiskFactorConfigurationStrategy
{
    public RiskFactorConfigurationLevel Level => RiskFactorConfigurationLevel.BuildingType;
    public IEnumerable<Adjustment> GetAdjustments(Building building, IEnumerable<RiskFactorConfiguration> configurations)
    {
        var applicableAdjustments = configurations
            .Where(c => c.BuildingType.ToString() == building.BuildingType)
            .Select(c => new Adjustment() { Type = AdjustmentTypeEnum.RiskFactorConfiguration, SubType = Level.ToString(), Percentage = c.AdjustmentPercentage });
        return applicableAdjustments;
    }
}