using InsuranceApp.Application.Common.Pagination;
using InsuranceApp.Application.Policies.DTOs;
using InsuranceApp.Domain.Entities;

namespace InsuranceApp.Application.Common.Repository;
public interface IPolicyRepository : IRepository<Policy, string>
{
    Task<PagedResult<PolicyDto>> GetAllPoliciesAsync(PageRequest pageRequest, PolicyFilter? filter, CancellationToken ct = default);
    Task<bool> ExistsByPolicyNumberAsync(string policyNumber, CancellationToken ct);
    Task<Policy?> GetDetailedPolicyByIdAsync(string policyNumber, CancellationToken ct);
}

