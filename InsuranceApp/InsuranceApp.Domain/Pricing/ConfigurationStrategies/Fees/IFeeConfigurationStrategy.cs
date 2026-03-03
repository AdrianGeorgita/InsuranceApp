using InsuranceApp.Domain.Entities;
using InsuranceApp.Domain.Enums;

namespace InsuranceApp.Domain.Pricing.ConfigurationStrategies.Fees;

public interface IFeeConfigurationStrategy
{
    FeeConfigurationType Type { get; }

    IEnumerable<decimal> GetAdjustments(Building building,
        IEnumerable<FeeConfiguration> configurations);
}