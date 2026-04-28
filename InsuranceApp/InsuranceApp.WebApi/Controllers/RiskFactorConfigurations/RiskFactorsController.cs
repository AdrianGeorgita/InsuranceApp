using InsuranceApp.Application.Common.Constants;
using InsuranceApp.Application.Common.Pagination;
using InsuranceApp.Application.Metadata.RiskFactorConfigurations;
using InsuranceApp.Application.Metadata.RiskFactorConfigurations.DTOs;
using InsuranceApp.WebApi.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InsuranceApp.WebApi.Controllers.RiskFactorConfigurations;

[ApiController]
[Authorize(Roles = AppRoles.Admin)]
[Route("api/admin/risk-factors")]
public class RiskFactorsController(IRiskFactorConfigurationService riskFactorService) : BaseApiController
{
    [HttpGet("", Name = "ListAllRiskFactorsAsync")]
    [ProducesResponseType(typeof(PagedResult<RiskFactorConfigurationDto>), 200)]
    [EndpointSummary("Get a paged list of risk factor configurations")]
    [EndpointDescription("Retrieves a paged list of risk factor configurations according to the passed pageNumber and pageSize.")]
    public async Task<ActionResult<PagedResult<RiskFactorConfigurationDto>>> ListAllRiskFactorsAsync([FromQuery] PageRequest pageRequest,
        CancellationToken ct)
    {
        var result = await riskFactorService.ListAllRiskFactorConfigurationAsync(pageRequest, ct);
        return FromResult(result);
    }

    [HttpGet("{riskFactorId:guid}", Name = "GetRiskFactorByIdAsync")]
    [ProducesResponseType(typeof(RiskFactorConfigurationDto), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [EndpointSummary("Get risk factor configuration by Id")]
    [EndpointDescription("Retrieves a single risk factor configuration using its unique identifier.")]
    public async Task<ActionResult<RiskFactorConfigurationDto>> GetRiskFactorByIdAsync(Guid riskFactorId, CancellationToken ct)
    {
        var result = await riskFactorService.GetRiskFactorConfigurationByIdAsync(riskFactorId, ct);
        return FromResult(result);
    }

    [HttpPost("", Name = "CreateRiskFactorAsync")]
    [ProducesResponseType(typeof(Guid), 201)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 409)]
    [EndpointSummary("Create a new risk factor configuration")]
    [EndpointDescription("Creates a new risk factor configuration and assigns it an unique identifier.")]
    [UnitOfWork]
    public async Task<ActionResult<Guid>> CreateRiskFactorAsync(CreateRiskFactorConfigurationRequest createRiskFactorDto, CancellationToken ct)
    {
        var result = await riskFactorService.CreateRiskFactorConfigurationAsync(createRiskFactorDto, ct);
        return FromCreated(result, "GetRiskFactorByIdAsync", id => new { riskFactorId = id });
    }

    [HttpPatch("{riskFactorId:guid}", Name = "UpdateRiskFactorAsync")]
    [ProducesResponseType(typeof(Guid), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 409)]
    [EndpointSummary("Update a risk factor configuration")]
    [EndpointDescription("Updates a risk factor configuration using its unique identifier.")]
    [UnitOfWork]
    public async Task<ActionResult<Guid>> UpdateRiskFactorAsync(Guid riskFactorId, UpdateRiskFactorConfigurationRequest updateRiskFactorDto, CancellationToken ct)
    {
        var result = await riskFactorService.UpdateRiskFactorConfigurationAsync(riskFactorId, updateRiskFactorDto, ct);
        return FromResult(result);
    }
}