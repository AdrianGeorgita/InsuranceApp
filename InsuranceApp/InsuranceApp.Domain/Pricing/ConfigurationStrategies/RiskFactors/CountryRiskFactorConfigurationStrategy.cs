using InsuranceApp.Domain.Entities;
using InsuranceApp.Domain.Enums;

namespace InsuranceApp.Domain.Pricing.ConfigurationStrategies.RiskFactors;

public class CountryRiskFactorConfigurationStrategy : IRiskFactorConfigurationStrategy
{
    public RiskFactorConfigurationLevel Level => RiskFactorConfigurationLevel.Country;
    public IEnumerable<decimal> GetAdjustments(Building building, IEnumerable<RiskFactorConfiguration> configurations)
    {
        var applicableAdjustments = configurations
            .Where(c => c.ReferenceId == building.City.County.CountryId)
            .Select(c => c.AdjustmentPercentage);
        return applicableAdjustments;
    }
}