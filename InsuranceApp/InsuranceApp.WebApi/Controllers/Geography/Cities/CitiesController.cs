using Asp.Versioning;
using InsuranceApp.Application.Common.Constants;
using InsuranceApp.Application.Geography.Cities;
using InsuranceApp.Application.Geography.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InsuranceApp.WebApi.Controllers.Geography.Cities;

[ApiController]
[Authorize(Roles = AppRoles.Broker)]
[Route("api/v{version:apiVersion}/brokers/[controller]")]
[ApiVersion("1.0")]
public class CitiesController(ICityService cityService) : BaseApiController
{
    [HttpGet("{cityId:guid}", Name = "GetCityByIdAsync")]
    [ProducesResponseType(typeof(CityDto), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [EndpointSummary("Get city by Id")]
    [EndpointDescription("Retrieves a single city using its unique identifier.")]
    public async Task<ActionResult<CityDto>> GetCityByIdAsync(Guid cityId, CancellationToken ct)
    {
        var result = await cityService.GetCityByIdAsync(cityId, ct);
        return FromResult(result);
    }
}

