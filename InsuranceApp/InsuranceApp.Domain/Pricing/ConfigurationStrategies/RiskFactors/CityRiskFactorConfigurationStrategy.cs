using InsuranceApp.Domain.Entities;
using InsuranceApp.Domain.Enums;

namespace InsuranceApp.Domain.Pricing.ConfigurationStrategies.RiskFactors;

public class CityRiskFactorConfigurationStrategy : IRiskFactorConfigurationStrategy
{
    public RiskFactorConfigurationLevel Level => RiskFactorConfigurationLevel.City;
    public IEnumerable<decimal> GetAdjustments(Building building, IEnumerable<RiskFactorConfiguration> configurations)
    {
        var applicableAdjustments = configurations
            .Where(c => c.ReferenceId == building.CityId)
            .Select(c => c.AdjustmentPercentage);
        return applicableAdjustments;
    }
}