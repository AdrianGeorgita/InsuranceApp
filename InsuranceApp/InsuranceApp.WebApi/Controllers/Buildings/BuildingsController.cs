using InsuranceApp.Application.Buildings;
using InsuranceApp.Application.Buildings.DTOs;
using InsuranceApp.Application.Common.Constants;
using InsuranceApp.WebApi.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InsuranceApp.WebApi.Controllers.Buildings;

[ApiController]
[Authorize(Roles = AppRoles.Broker)]
[Route("api/brokers/[controller]")]
public class BuildingsController(IBuildingService buildingService) : BaseApiController
{
    [HttpGet("{buildingId:guid}", Name = "GetBuildingByIdAsync")]
    [ProducesResponseType(typeof(BuildingDto), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [EndpointSummary("Get building by Id")]
    [EndpointDescription("Retrieves a single building using its unique identifier.")]
    public async Task<ActionResult<BuildingDto>> GetBuildingByIdAsync(Guid buildingId, CancellationToken ct)
    {
        var result = await buildingService.GetBuildingByIdAsync(buildingId, ct);
        return FromResult(result);
    }

    [HttpPatch("{buildingId:guid}", Name = "UpdateBuildingByIdAsync")]
    [ProducesResponseType(typeof(Guid), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [EndpointSummary("Update a building")]
    [EndpointDescription("Updates a building and its risk indicators using its unique identifier.")]
    [UnitOfWork]
    public async Task<ActionResult<Guid>> UpdateClientAsync(Guid buildingId, UpdateBuildingRequest updateBuildingDto, CancellationToken ct)
    {
        var result = await buildingService.UpdateBuildingAsync(buildingId, updateBuildingDto, ct);
        return FromResult(result);
    }

    [HttpDelete("{buildingId:guid}", Name = "DeleteBuildingByIdAsync")]
    [ProducesResponseType(typeof(Guid), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [EndpointSummary("Delete a building")]
    [EndpointDescription("Soft deletes a building using its unique identifier.")]
    [UnitOfWork]
    public async Task<ActionResult<Guid>> DeleteBuildingByIdAsync(Guid buildingId, CancellationToken ct)
    {
        var result = await buildingService.DeleteBuildingByIdAsync(buildingId, ct);
        return FromResult(result);
    }
}