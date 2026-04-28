using InsuranceApp.Application.Common.Constants;
using InsuranceApp.Application.Common.Pagination;
using InsuranceApp.Application.Geography.Cities;
using InsuranceApp.Application.Geography.Counties;
using InsuranceApp.Application.Geography.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InsuranceApp.WebApi.Controllers.Geography.Counties;

[ApiController]
[Authorize(Roles = AppRoles.Broker)]
[Route("api/brokers/[controller]")]
public class CountiesController(ICountyService countyService, ICityService cityService) : BaseApiController
{
    [HttpGet("{countyId:guid}", Name = "GetCountyByIdAsync")]
    [ProducesResponseType(typeof(CountyDto), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [EndpointSummary("Get county by Id")]
    [EndpointDescription("Retrieves a single county using its unique identifier.")]
    public async Task<ActionResult<CountyDto>> GetCountyByIdAsync(Guid countyId, CancellationToken ct)
    {
        var result = await countyService.GetCountyByIdAsync(countyId, ct);
        return FromResult(result);
    }

    [HttpGet("{countyId:guid}/cities", Name = "ListAllCountyCitiesAsync")]
    [ProducesResponseType(typeof(PagedResult<CityDto>), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [EndpointSummary("Get a paged list of cities of a county")]
    [EndpointDescription("Retrieves a paged list of cities linked to a county, according to the passed pageNumber and pageSize.")]
    public async Task<ActionResult<PagedResult<CityDto>>> ListAllCountyCitiesAsync([FromQuery] PageRequest pageRequest, Guid countyId, CancellationToken ct)
    {
        var result = await cityService.ListAllCountyCitiesAsync(pageRequest, countyId, ct);
        return FromResult(result);
    }
}

