using InsuranceApp.Application.Common.Constants;
using InsuranceApp.Application.Common.Pagination;
using InsuranceApp.Application.Policies;
using InsuranceApp.Application.Policies.DTOs;
using InsuranceApp.Domain.Enums;
using InsuranceApp.WebApi.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InsuranceApp.WebApi.Controllers.Policies;

[ApiController]
[Authorize(Roles = AppRoles.Broker)]
[Route("api/brokers/[controller]")]
public class PoliciesController(IPolicyService policyService) : BaseApiController
{
    [HttpGet("", Name = "ListAllPoliciesAsync")]
    [ProducesResponseType(typeof(PagedResult<PolicyDto>), 200)]
    [EndpointSummary("Get a paged list of policies")]
    [EndpointDescription("Retrieves a paged list of policies according to the passed pageNumber, pageSize and filter")]
    public async Task<ActionResult<PagedResult<PolicyDto>>> ListAllPoliciesAsync([FromQuery] PageRequest pageRequest,
        [FromQuery] PolicyFilter? filter, CancellationToken ct)
    {
        var result = await policyService.ListAllPoliciesAsync(pageRequest, filter, ct);
        return FromResult(result);
    }

    [HttpGet("{policyNumber}", Name = "GetPolicyByIdAsync")]
    [ProducesResponseType(typeof(DetailedPolicyDto), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [EndpointSummary("Get policy details by Id")]
    [EndpointDescription("Retrieves a single detailed policy using its unique identifier.")]
    public async Task<ActionResult<DetailedPolicyDto>> GetPolicyByIdAsync(string policyNumber, CancellationToken ct)
    {
        var result = await policyService.GetPolicyByIdAsync(policyNumber, ct);
        return FromResult(result);
    }

    [HttpDelete("{policyNumber}", Name = "DeletePolicyByIdAsync")]
    [ProducesResponseType(typeof(string), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [EndpointSummary("Delete policy by Id")]
    [EndpointDescription("Soft deletes a policy using its unique identifier.")]
    [UnitOfWork]
    public async Task<ActionResult<string>> DeletePolicyByIdAsync(string policyNumber, CancellationToken ct)
    {
        var result = await policyService.DeletePolicyByIdAsync(policyNumber, ct);
        return FromResult(result);
    }

    [HttpPost("", Name = "CreatePolicyAsync")]
    [ProducesResponseType(typeof(string), 201)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 409)]
    [EndpointSummary("Create a new policy")]
    [EndpointDescription("Creates a new policy with a DRAFT status and assigns it an unique identifier.")]
    [UnitOfWork]
    public async Task<ActionResult<string>> CreatePolicyAsync(CreatePolicyRequest createPolicyDto, CancellationToken ct)
    {
        var result = await policyService.CreatePolicyAsync(createPolicyDto, ct);
        return FromCreated(result, "GetPolicyByIdAsync", id => new { policyNumber = id });
    }

    [HttpPost("{policyNumber}/activate", Name = "ActivatePolicyByIdAsync")]
    [ProducesResponseType(typeof(string), 201)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [EndpointSummary("Activate a policy")]
    [EndpointDescription("Updates the status of a policy from DRAFT to ACTIVE by its unique identifier.")]
    [UnitOfWork]
    public async Task<ActionResult<string>> ActivatePolicyByIdAsync(string policyNumber, CancellationToken ct)
    {
        var result = await policyService.UpdatePolicyStatusAsync(policyNumber, new UpdatePolicyRequest { NewStatus = PolicyStatus.Active }, ct);
        return FromCreated(result, "GetPolicyByIdAsync", id => new { policyNumber = id });
    }

    [HttpPost("{policyNumber}/cancel", Name = "CancelPolicyByIdAsync")]
    [ProducesResponseType(typeof(string), 201)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [EndpointSummary("Cancels a policy")]
    [EndpointDescription("Updates the status of a policy from ACTIVE to CANCELLED by its unique identifier, with a specified reason.")]
    [UnitOfWork]
    public async Task<ActionResult<string>> CancelPolicyByIdAsync(string policyNumber, CancelPolicyRequest cancelPolicyDto, CancellationToken ct)
    {
        var result = await policyService.UpdatePolicyStatusAsync(policyNumber, new UpdatePolicyRequest { NewStatus = PolicyStatus.Cancelled, Reason = cancelPolicyDto.Reason }, ct);
        return FromCreated(result, "GetPolicyByIdAsync", id => new { policyNumber = id });
    }
}