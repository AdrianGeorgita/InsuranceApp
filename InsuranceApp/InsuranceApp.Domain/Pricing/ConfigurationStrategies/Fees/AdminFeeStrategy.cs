using InsuranceApp.Domain.Entities;
using InsuranceApp.Domain.Enums;

namespace InsuranceApp.Domain.Pricing.ConfigurationStrategies.Fees;

public class AdminFeeStrategy : IFeeConfigurationStrategy
{
    public FeeConfigurationType Type => FeeConfigurationType.AdminFee;
    public IEnumerable<decimal> GetAdjustments(Building building, IEnumerable<FeeConfiguration> configurations)
    {
        var applicableAdjustments = configurations
            .Select(c => c.Percentage);
        return applicableAdjustments;
    }
}