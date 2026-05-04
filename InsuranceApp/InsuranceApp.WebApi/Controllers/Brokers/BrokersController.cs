using Asp.Versioning;
using InsuranceApp.Application.Brokers;
using InsuranceApp.Application.Brokers.DTOs;
using InsuranceApp.Application.Common.Constants;
using InsuranceApp.Application.Common.Pagination;
using InsuranceApp.Domain.Enums;
using InsuranceApp.WebApi.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InsuranceApp.WebApi.Controllers.Brokers;

[ApiController]
[Authorize(Roles = AppRoles.Admin)]
[Route("api/v{version:apiVersion}/admin/[controller]")]
[ApiVersion("1.0")]
public class BrokersController(IBrokerService brokerService) : BaseApiController
{
    [HttpGet("", Name = "ListAllBrokersAsync")]
    [ProducesResponseType(typeof(PagedResult<BrokerDto>), 200)]
    [EndpointSummary("Get a paged list of brokers")]
    [EndpointDescription("Retrieves a paged list of brokers according to the passed pageNumber and pageSize.")]
    public async Task<ActionResult<PagedResult<BrokerDto>>> ListAllBrokersAsync([FromQuery] PageRequest pageRequest,
        CancellationToken ct)
    {
        var result = await brokerService.ListAllBrokersAsync(pageRequest, ct);
        return FromResult(result);
    }

    [HttpGet("{brokerId:guid}", Name = "GetBrokerByIdAsync")]
    [ProducesResponseType(typeof(BrokerDto), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [EndpointSummary("Get broker by Id")]
    [EndpointDescription("Retrieves a single broker using its unique identifier.")]
    public async Task<ActionResult<BrokerDto>> GetBrokerByIdAsync(Guid brokerId, CancellationToken ct)
    {
        var result = await brokerService.GetBrokerByIdAsync(brokerId, ct);
        return FromResult(result);
    }

    [HttpPost("", Name = "CreateBrokerAsync")]
    [ProducesResponseType(typeof(Guid), 201)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 409)]
    [EndpointSummary("Create a new broker")]
    [EndpointDescription("Creates a new broker and assigns it an unique identifier.")]
    [UnitOfWork]
    public async Task<ActionResult<Guid>> CreateBrokerAsync(CreateBrokerRequest createBrokerDto, CancellationToken ct)
    {
        var result = await brokerService.CreateBrokerAsync(createBrokerDto, ct);
        return FromCreated(result, "GetBrokerByIdAsync", id => new { brokerId = id });
    }

    [HttpPatch("{brokerId:guid}", Name = "UpdateBrokerAsync")]
    [ProducesResponseType(typeof(Guid), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 409)]
    [EndpointSummary("Update a broker")]
    [EndpointDescription("Updates a broker using its unique identifier.")]
    [UnitOfWork]
    public async Task<ActionResult<Guid>> UpdateBrokerAsync(Guid brokerId, UpdateBrokerRequest updateBrokerDto, CancellationToken ct)
    {
        var result = await brokerService.UpdateBrokerAsync(brokerId, updateBrokerDto, ct);
        return FromResult(result);
    }

    [HttpDelete("{brokerId:guid}", Name = "DeleteBrokerByIdAsync")]
    [ProducesResponseType(typeof(Guid), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [EndpointSummary("Delete broker by Id")]
    [EndpointDescription("Soft deletes a broker using its unique identifier.")]
    [UnitOfWork]
    public async Task<ActionResult<Guid>> DeleteBrokerByIdAsync(Guid brokerId, CancellationToken ct)
    {
        var result = await brokerService.DeleteBrokerAsync(brokerId, ct);
        return FromResult(result);
    }

    [HttpPost("{brokerId:guid}/activate", Name = "ActivateBrokerByIdAsync")]
    [ProducesResponseType(typeof(Guid), 201)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [EndpointSummary("Activate a broker")]
    [EndpointDescription("Updates the status of a broker to ACTIVE by its unique identifier.")]
    [UnitOfWork]
    public async Task<ActionResult<Guid>> ActivateBrokerByIdAsync(Guid brokerId, CancellationToken ct)
    {
        var result = await brokerService.UpdateBrokerStatusAsync(brokerId, BrokerStatus.Active, ct);
        return FromCreated(result, "GetBrokerByIdAsync", id => new { brokerId = id });
    }

    [HttpPost("{brokerId:guid}/deactivate", Name = "DeactivateBrokerByIdAsync")]
    [ProducesResponseType(typeof(Guid), 201)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [EndpointSummary("Deactivate a broker")]
    [EndpointDescription("Updates the status of a broker to INACTIVE by its unique identifier.")]
    [UnitOfWork]
    public async Task<ActionResult<Guid>> DeactivateBrokerByIdAsync(Guid brokerId, CancellationToken ct)
    {
        var result = await brokerService.UpdateBrokerStatusAsync(brokerId, BrokerStatus.Inactive, ct);
        return FromCreated(result, "GetBrokerByIdAsync", id => new { brokerId = id });
    }
}