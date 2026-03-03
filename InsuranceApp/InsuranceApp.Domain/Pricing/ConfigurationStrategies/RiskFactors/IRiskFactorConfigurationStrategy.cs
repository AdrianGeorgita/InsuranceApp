using InsuranceApp.Domain.Entities;
using InsuranceApp.Domain.Enums;

namespace InsuranceApp.Domain.Pricing.ConfigurationStrategies.RiskFactors;

public interface IRiskFactorConfigurationStrategy
{
    RiskFactorConfigurationLevel Level { get; }

    IEnumerable<decimal> GetAdjustments(Building building,
        IEnumerable<RiskFactorConfiguration> configurations);
}