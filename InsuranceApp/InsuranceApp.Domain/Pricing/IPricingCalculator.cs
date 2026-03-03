using InsuranceApp.Domain.Entities;

namespace InsuranceApp.Domain.Pricing;

public interface IPricingCalculator
{
    public IEnumerable<decimal> GetAllApplicableAdjustments(Building building, Broker broker,
        IEnumerable<RiskFactorConfiguration> riskFactorConfigurations, IEnumerable<FeeConfiguration> feeConfigurations);
}