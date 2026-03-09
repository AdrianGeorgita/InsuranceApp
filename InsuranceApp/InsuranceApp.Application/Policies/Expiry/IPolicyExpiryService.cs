using FluentResults;

namespace InsuranceApp.Application.Policies.Expiry;

public interface IPolicyExpiryService
{
    Task<Result<int>> MarkExpiredPoliciesAsync(CancellationToken ct);
}