using InsuranceApp.Domain.Entities;
using InsuranceApp.Domain.Models;

namespace InsuranceApp.Domain.Pricing;

public interface IPricingCalculator
{
    public IEnumerable<Adjustment> GetAllApplicableAdjustments(Building building, Broker broker,
        IEnumerable<RiskFactorConfiguration> riskFactorConfigurations, IEnumerable<FeeConfiguration> feeConfigurations);
}