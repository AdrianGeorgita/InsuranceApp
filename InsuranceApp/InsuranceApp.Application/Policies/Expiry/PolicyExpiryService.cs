using FluentResults;
using InsuranceApp.Application.Common.Persistence;
using InsuranceApp.Application.Common.Repository;
using InsuranceApp.Domain.Entities;
using InsuranceApp.Domain.Enums;

namespace InsuranceApp.Application.Policies.Expiry;

public class PolicyExpiryService(IPolicyRepository policyRepository, IUnitOfWork uow) : IPolicyExpiryService
{
    public async Task<Result<int>> MarkExpiredPoliciesAsync(CancellationToken ct)
    {
        var expiredPolicies = await policyRepository.GetExpiredPoliciesAsync(ct);
        var updatedPolicies = MarkPoliciesAsExpired(expiredPolicies);
        await uow.SaveChangesAsync(ct);
        return Result.Ok(updatedPolicies);
    }

    private static int MarkPoliciesAsExpired(IEnumerable<Policy> policies)
    {
        var updatedPolicies = 0;
        foreach (var policy in policies)
        {
            updatedPolicies++;
            policy.Status = PolicyStatus.Expired;
        }
        return updatedPolicies;
    }
}