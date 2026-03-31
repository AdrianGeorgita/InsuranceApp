using AutoMapper;
using FluentResults;
using InsuranceApp.Application.Common.Audit;
using InsuranceApp.Application.Common.Errors;
using InsuranceApp.Application.Common.Messaging;
using InsuranceApp.Application.Common.Pagination;
using InsuranceApp.Application.Common.Repository;
using InsuranceApp.Application.Common.Validation;
using InsuranceApp.Application.Policies.DTOs;
using InsuranceApp.Application.Policies.Pricing;
using InsuranceApp.Domain.Entities;
using InsuranceApp.Domain.Enums;
using InsuranceApp.Domain.Premiums;
using InsuranceApp.Domain.Pricing;
using Microsoft.Extensions.Logging;

namespace InsuranceApp.Application.Policies;

public class PolicyService(IPolicyRepository policyRepository, IPriceFetchService priceFetchService, IRequestValidator requestValidator,
    IMapper mapper, IAuditEventPublisher auditEventPublisher, ILogger<PolicyService> logger,
    IPolicyEventPublisher eventPublisher, IPricingCalculator pricingCalculator) : IPolicyService
{
    public async Task<Result<PagedResult<PolicyDto>>> ListAllPoliciesAsync(PageRequest pageRequest, PolicyFilter? filter, CancellationToken ct)
    {
        var pagedResult = await policyRepository.GetAllPoliciesAsync(pageRequest, filter, ct);
        return Result.Ok(pagedResult);
    }

    public async Task<Result<DetailedPolicyDto>> GetPolicyByIdAsync(string policyNumber, CancellationToken ct)
    {
        var policy = await policyRepository.GetDetailedPolicyByIdAsync(policyNumber, ct);

        if (policy is null)
            return Result.Fail<DetailedPolicyDto>(new NotFoundError($"Policy '{policyNumber}' not found."));

        return Result.Ok(mapper.Map<DetailedPolicyDto>(policy));
    }

    public async Task<Result<string>> CreatePolicyAsync(CreatePolicyRequest createPolicyDto, CancellationToken ct)
    {
        var requestValidationResult = await EnsureValidRequestAndNoConflictAsync(createPolicyDto, createPolicyDto.ClientId,
            createPolicyDto.BrokerId, createPolicyDto.BuildingId, ct);
        if (requestValidationResult.IsFailed) return requestValidationResult;

        var policy = mapper.Map<Policy>(createPolicyDto);

        policy.PolicyNumber = $"POLICY-{Guid.NewGuid():N}";
        policy.Status = PolicyStatus.Draft;
        policy.CreatedAt = DateTime.UtcNow;
        policy.UpdatedAt = DateTime.UtcNow;

        var finalPremium = await GetFinalPremium(policy, policy.BrokerId, ct);
        var premiumValidationResult = ValidateFinalPremium(finalPremium);
        if (premiumValidationResult.IsFailed) return premiumValidationResult;

        policy.FinalPremium = finalPremium;

        logger.LogInformation("Policy with number '{PolicyNumber}' has been created.", policy.PolicyNumber);

        await policyRepository.AddAsync(policy, ct);

        await eventPublisher.PublishPolicyEventAsync(policy, CancellationToken.None);

        return Result.Ok(policy.PolicyNumber);
    }

    public async Task<Result<string>> UpdatePolicyStatusAsync(string policyNumber, UpdatePolicyRequest updateDto, CancellationToken ct)
    {
        var existingPolicy = await policyRepository.GetDetailedPolicyByIdAsync(policyNumber, ct);
        if (existingPolicy is null)
            return Result.Fail<string>(new NotFoundError($"Policy '{policyNumber}' not found."));

        var requestValidationResult = await EnsureCanUpdatePolicyStatusAsync(updateDto, existingPolicy, updateDto.NewStatus, ct);
        if (requestValidationResult.IsFailed) return requestValidationResult;

        await LogPolicyStatusChange(existingPolicy.Status,
                updateDto.NewStatus, existingPolicy.PolicyNumber, existingPolicy.BrokerId, updateDto.Reason, ct);

        existingPolicy.Status = updateDto.NewStatus;
        existingPolicy.UpdatedAt = DateTime.UtcNow;

        logger.LogInformation("The status of the policy with number '{PolicyNumber}' has been updated to {NewPolicyStatus}.", existingPolicy.PolicyNumber, updateDto.NewStatus);

        await eventPublisher.PublishPolicyEventAsync(existingPolicy, ct);
        return Result.Ok(existingPolicy.PolicyNumber);
    }

    public async Task<Result<string>> DeletePolicyByIdAsync(string policyNumber, CancellationToken ct)
    {
        var existingPolicy = await policyRepository.GetDetailedPolicyByIdAsync(policyNumber, ct);
        if (existingPolicy is null)
            return Result.Fail<string>(new NotFoundError($"Policy '{policyNumber}' not found."));

        policyRepository.Remove(existingPolicy);

        logger.LogInformation("Policy with number '{PolicyNumber}' has been deleted.", existingPolicy.PolicyNumber);
        return Result.Ok(existingPolicy.PolicyNumber);
    }

    private async Task LogPolicyStatusChange(PolicyStatus oldStatus, PolicyStatus newStatus,
        string policyNumber, Guid brokerId, string? reason, CancellationToken ct)
    {
        var auditEventId = Guid.NewGuid();
        var auditChangeEvent = new AuditTableChangeEvent
        {
            EventId = auditEventId,
            UserId = brokerId,
            TableName = "Policies",
            ColumnName = "Status",
            OldValue = oldStatus.ToString(),
            NewValue = newStatus.ToString(),
            RowId = policyNumber,
            Reason = reason
        };

        await auditEventPublisher.PublishAuditEventAsync(auditChangeEvent, ct);
    }

    private async Task<Result<string>> EnsureCanUpdatePolicyStatusAsync<TRequest>(TRequest request, Policy policy, PolicyStatus newStatus, CancellationToken ct)
    {
        var validation = await requestValidator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return Result.Fail(validation.ToApiError());

        if (!IsValidPolicyStatusTransition(policy, newStatus))
            return Result.Fail<string>(new ValidationError($"Cannot change Policy Status from {policy.Status} to {newStatus}."));

        return Result.Ok();
    }

    private static bool IsValidPolicyStatusTransition(Policy policy, PolicyStatus newStatus)
    {
        return newStatus switch
        {
            PolicyStatus.Active when policy.Status == PolicyStatus.Draft && policy.StartDate >= DateTime.UtcNow => true,
            PolicyStatus.Cancelled when policy.Status == PolicyStatus.Active => true,
            _ => false
        };
    }

    private async Task<Result> EnsureValidRequestAndNoConflictAsync<TRequest>(TRequest request, Guid clientId,
        Guid brokerId, Guid buildingId, CancellationToken ct)
    {
        var validation = await requestValidator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return Result.Fail(validation.ToApiError());

        if (await policyRepository.ExistsAsync(p => p.BrokerId == brokerId &&
                                                    p.ClientId == clientId &&
                                                    p.BuildingId == buildingId, ct))
            return Result.Fail(new ConflictError(
                $"Policy with ClientId: '{clientId}', BrokerId: '{brokerId}' and BuildingId: '{buildingId}' already exists."));

        return Result.Ok();
    }

    private async Task<decimal> GetFinalPremium(Policy policy, Guid brokerId, CancellationToken ct)
    {
        var pricingContext = await priceFetchService.FetchPricingContextAsync(policy.BuildingId, policy.StartDate, brokerId, ct);
        var adjustments = pricingCalculator.GetAllApplicableAdjustments(pricingContext.Building, pricingContext.Broker,
            pricingContext.RiskFactorConfigurations, pricingContext.FeeConfigurations);
        var finalPremium = PremiumCalculator.Calculate(policy.BasePremium, adjustments);
        return finalPremium;
    }

    private static Result<string> ValidateFinalPremium(decimal finalPremium) => finalPremium <= 0
        ? Result.Fail(new ValidationError("FinalPremium cannot be less than or equal to 0."))
        : Result.Ok();
}