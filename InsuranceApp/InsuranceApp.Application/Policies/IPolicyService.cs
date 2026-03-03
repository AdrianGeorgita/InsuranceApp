using FluentResults;
using InsuranceApp.Application.Common.Pagination;
using InsuranceApp.Application.Policies.DTOs;
using InsuranceApp.Domain.Enums;

namespace InsuranceApp.Application.Policies;

public interface IPolicyService
{
    Task<Result<PagedResult<PolicyDto>>> ListAllPoliciesAsync(PageRequest pageRequest, PolicyFilter? filter, CancellationToken ct);
    Task<Result<DetailedPolicyDto>> GetPolicyByIdAsync(string policyNumber, CancellationToken ct);

    Task<Result<string>> CreatePolicyAsync(CreatePolicyRequest createPolicyDto, CancellationToken ct);
    Task<Result<string>> UpdatePolicyStatusAsync(string policyNumber, UpdatePolicyRequest updateDto, CancellationToken ct);
}