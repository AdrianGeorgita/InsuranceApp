using InsuranceApp.Domain.Entities;

namespace InsuranceApp.Application.Policies.DTOs;

public class PricingContextDto
{
    public Building Building { get; init; } = null!;
    public IEnumerable<RiskFactorConfiguration> RiskFactorConfigurations { get; init; } = [];
    public IEnumerable<FeeConfiguration> FeeConfigurations { get; init; } = [];
    public Broker Broker { get; init; } = null!;
}