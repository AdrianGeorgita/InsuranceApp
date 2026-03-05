using InsuranceApp.Domain.Entities;
using InsuranceApp.Domain.Enums;
using InsuranceApp.Domain.Models;

namespace InsuranceApp.Domain.Pricing.ConfigurationStrategies.RiskFactors;

public interface IRiskFactorConfigurationStrategy
{
    RiskFactorConfigurationLevel Level { get; }

    IEnumerable<Adjustment> GetAdjustments(Building building,
        IEnumerable<RiskFactorConfiguration> configurations);
}