using InsuranceApp.Domain.Entities;
using InsuranceApp.Domain.Enums;
using InsuranceApp.Domain.Models;

namespace InsuranceApp.Domain.Pricing.ConfigurationStrategies.Fees;

public class BrokerCommissionFeeStrategy : IFeeConfigurationStrategy
{
    public FeeConfigurationType Type => FeeConfigurationType.BrokerCommission;
    public IEnumerable<Adjustment> GetAdjustments(Building building, IEnumerable<FeeConfiguration> configurations)
    {
        var applicableAdjustments = configurations
            .Select(c => new Adjustment() { Type = AdjustmentTypeEnum.FeeConfiguration, SubType = Type.ToString(), Percentage = c.Percentage });
        return applicableAdjustments;
    }
}