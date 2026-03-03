using InsuranceApp.Application.Policies.DTOs;

namespace InsuranceApp.Application.Policies.Pricing;

public interface IPriceFetchService
{
    public Task<PricingContextDto> FetchPricingContextAsync(Guid policyBuildingId, DateTime policyStartDate, Guid brokerId, CancellationToken ct);
}