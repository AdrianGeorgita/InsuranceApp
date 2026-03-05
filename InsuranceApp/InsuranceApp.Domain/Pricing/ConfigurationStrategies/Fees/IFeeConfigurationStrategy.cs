using InsuranceApp.Domain.Entities;
using InsuranceApp.Domain.Enums;
using InsuranceApp.Domain.Models;

namespace InsuranceApp.Domain.Pricing.ConfigurationStrategies.Fees;

public interface IFeeConfigurationStrategy
{
    FeeConfigurationType Type { get; }

    IEnumerable<Adjustment> GetAdjustments(Building building,
        IEnumerable<FeeConfiguration> configurations);
}