using Asp.Versioning;
using InsuranceApp.Application.Buildings.DTOs;
using InsuranceApp.Application.Clients;
using InsuranceApp.Application.Clients.Buildings;
using InsuranceApp.Application.Clients.DTOs;
using InsuranceApp.Application.Common.Constants;
using InsuranceApp.Application.Common.Pagination;
using InsuranceApp.WebApi.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InsuranceApp.WebApi.Controllers.Clients;

[ApiController]
[Authorize(Roles = AppRoles.Broker)]
[Route("api/v{version:apiVersion}/brokers/[controller]")]
[ApiVersion("1.0")]
public class ClientsController(IClientService clientService, IClientBuildingService clientBuildingService) : BaseApiController
{
    [HttpGet("", Name = "ListAllClientsAsync")]
    [ProducesResponseType(typeof(PagedResult<ClientDto>), 200)]
    [EndpointSummary("Get a paged list of clients")]
    [EndpointDescription("Retrieves a filtered paged list of clients according to the passed pageNumber and pageSize and filters applied.")]
    public async Task<ActionResult<PagedResult<ClientDto>>> ListAllClientsAsync([FromQuery] PageRequest pageRequest,
        [FromQuery] ClientFilter? filter, CancellationToken ct)
    {
        var result = await clientService.ListAllClientsAsync(pageRequest, filter, ct);
        return FromResult(result);
    }

    [HttpGet("{clientId:guid}", Name = "GetClientByIdAsync")]
    [ProducesResponseType(typeof(ClientDto), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [EndpointSummary("Get client by Id")]
    [EndpointDescription("Retrieves a single client using its unique identifier.")]
    public async Task<ActionResult<ClientDto>> GetClientByIdAsync(Guid clientId, CancellationToken ct)
    {
        var result = await clientService.GetClientByIdAsync(clientId, ct);
        return FromResult(result);
    }

    [HttpPost("", Name = "CreateClientAsync")]
    [ProducesResponseType(typeof(Guid), 201)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 409)]
    [EndpointSummary("Create a new client")]
    [EndpointDescription("Creates a new client and assigns it an unique identifier.")]
    [UnitOfWork]
    public async Task<ActionResult<Guid>> CreateClientAsync(CreateClientRequest createClientDto, CancellationToken ct)
    {
        var result = await clientService.CreateClientAsync(createClientDto, ct);
        return FromCreated(result, "GetClientByIdAsync", id => new { clientId = id });
    }

    [HttpPatch("{clientId:guid}", Name = "UpdateClientAsync")]
    [ProducesResponseType(typeof(Guid), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 409)]
    [EndpointSummary("Update a client")]
    [EndpointDescription("Updates a client using its unique identifier.")]
    [UnitOfWork]
    public async Task<ActionResult<Guid>> UpdateClientAsync(Guid clientId, UpdateClientRequest updateClientDto, CancellationToken ct)
    {
        var result = await clientService.UpdateClientAsync(clientId, updateClientDto, ct);
        return FromResult(result);
    }

    [HttpDelete("{clientId:guid}", Name = "DeleteClientByIdAsync")]
    [ProducesResponseType(typeof(Guid), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [EndpointSummary("Delete a client")]
    [EndpointDescription("Soft deletes a client using its unique identifier.")]
    [UnitOfWork]
    public async Task<ActionResult<Guid>> DeleteClientByIdAsync(Guid clientId, CancellationToken ct)
    {
        var result = await clientService.DeleteClientByIdAsync(clientId, ct);
        return FromResult(result);
    }

    [HttpGet("{clientId:guid}/buildings", Name = "ListAllClientBuildingsAsync")]
    [ProducesResponseType(typeof(PagedResult<BuildingDto>), 200)]
    [ProducesResponseType(typeof(PagedResult<ProblemDetails>), 404)]
    [EndpointSummary("Get a paged list of buildings for a client")]
    [EndpointDescription("Retrieves a paged list of buildings linked to the passed client unique identifier.")]
    public async Task<ActionResult<PagedResult<BuildingDto>>> ListAllClientBuildingsAsync(Guid clientId,
        [FromQuery] PageRequest pageRequest, CancellationToken ct)
    {
        var result = await clientBuildingService.ListAllClientBuildingsAsync(clientId, pageRequest, ct);
        return FromResult(result);
    }

    [HttpPost("{clientId:guid}/buildings", Name = "CreateClientBuildingAsync")]
    [ProducesResponseType(typeof(Guid), 201)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [EndpointSummary("Create a new building for a client")]
    [EndpointDescription("Creates a new building, assigns risk indicators to it and assigns it an unique identifier then links it to a client.")]
    [UnitOfWork]
    public async Task<ActionResult<Guid>> CreateClientBuildingAsync(Guid clientId, CreateBuildingRequest createBuildingDto, CancellationToken ct)
    {
        var result = await clientBuildingService.CreateClientBuildingAsync(clientId, createBuildingDto, ct);
        return FromCreated(result, "GetBuildingByIdAsync", id => new { buildingId = id });
    }
}