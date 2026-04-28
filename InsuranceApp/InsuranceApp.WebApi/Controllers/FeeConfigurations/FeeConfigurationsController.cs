using InsuranceApp.Application.Common.Constants;
using InsuranceApp.Application.Common.Pagination;
using InsuranceApp.Application.Metadata.FeeConfigurations;
using InsuranceApp.Application.Metadata.FeeConfigurations.DTOs;
using InsuranceApp.WebApi.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InsuranceApp.WebApi.Controllers.FeeConfigurations;

[ApiController]
[Authorize(Roles = AppRoles.Admin)]
[Route("api/admin/fees")]
public class FeeConfigurationsController(IFeeConfigurationService feeConfigurationService) : BaseApiController
{
    [HttpGet("", Name = "ListAllFeeConfigurationsAsync")]
    [ProducesResponseType(typeof(PagedResult<FeeConfigurationDto>), 200)]
    [EndpointSummary("Get a paged list of fee configurations")]
    [EndpointDescription("Retrieves a paged list of fee configurations according to the passed pageNumber and pageSize.")]
    public async Task<ActionResult<PagedResult<FeeConfigurationDto>>> ListAllFeeConfigurationsAsync([FromQuery] PageRequest pageRequest,
        CancellationToken ct)
    {
        var result = await feeConfigurationService.ListAllFeeConfigurationsAsync(pageRequest, ct);
        return FromResult(result);
    }

    [HttpGet("{feeId:guid}", Name = "GetFeeConfigurationByIdAsync")]
    [ProducesResponseType(typeof(FeeConfigurationDto), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [EndpointSummary("Get fee configuration by Id")]
    [EndpointDescription("Retrieves a single fee configuration using its unique identifier.")]
    public async Task<ActionResult<FeeConfigurationDto>> GetFeeConfigurationByIdAsync(Guid feeId, CancellationToken ct)
    {
        var result = await feeConfigurationService.GetFeeConfigurationByIdAsync(feeId, ct);
        return FromResult(result);
    }

    [HttpPost("", Name = "CreateFeeConfigurationAsync")]
    [ProducesResponseType(typeof(Guid), 201)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 409)]
    [EndpointSummary("Create a new fee configuration")]
    [EndpointDescription("Creates a new fee configuration and assigns it an unique identifier.")]
    [UnitOfWork]
    public async Task<ActionResult<Guid>> CreateFeeConfigurationAsync(CreateFeeConfigurationRequest createFeeDto, CancellationToken ct)
    {
        var result = await feeConfigurationService.CreateFeeConfigurationAsync(createFeeDto, ct);
        return FromCreated(result, "GetFeeConfigurationByIdAsync", id => new { feeId = id });
    }

    [HttpPatch("{feeId:guid}", Name = "UpdateFeeConfigurationAsync")]
    [ProducesResponseType(typeof(Guid), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 409)]
    [EndpointSummary("Update a fee configuration")]
    [EndpointDescription("Updates a fee configuration using its unique identifier.")]
    [UnitOfWork]
    public async Task<ActionResult<Guid>> UpdateFeeConfigurationAsync(Guid feeId, UpdateFeeConfigurationRequest updateFeeDto, CancellationToken ct)
    {
        var result = await feeConfigurationService.UpdateFeeConfigurationAsync(feeId, updateFeeDto, ct);
        return FromResult(result);
    }
}