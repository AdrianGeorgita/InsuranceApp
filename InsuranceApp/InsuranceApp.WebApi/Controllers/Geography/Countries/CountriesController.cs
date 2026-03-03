using InsuranceApp.Application.Common.Pagination;
using InsuranceApp.Application.Geography.Counties;
using InsuranceApp.Application.Geography.Countries;
using InsuranceApp.Application.Geography.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace InsuranceApp.WebApi.Controllers.Geography.Countries;

[ApiController]
[Route("api/brokers/[controller]")]
public class CountriesController(ICountryService countryService, ICountyService countyService) : BaseApiController
{
    [HttpGet("", Name = "ListAllCountriesAsync")]
    [ProducesResponseType(typeof(PagedResult<CountryDto>), 200)]
    [EndpointSummary("Get a paged list of countries")]
    [EndpointDescription("Retrieves a paged list of countries according to the passed pageNumber and pageSize.")]
    public async Task<ActionResult<PagedResult<CountryDto>>> ListAllCountriesAsync([FromQuery] PageRequest pageRequest, CancellationToken ct)
    {
        var result = await countryService.ListAllCountriesAsync(pageRequest, ct);
        return FromResult(result);
    }

    [HttpGet("{countryId:guid}", Name = "GetCountryByIdAsync")]
    [ProducesResponseType(typeof(CountryDto), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [EndpointSummary("Get country by Id")]
    [EndpointDescription("Retrieves a single country using its unique identifier.")]
    public async Task<ActionResult<CountryDto>> GetCountryByIdAsync(Guid countryId, CancellationToken ct)
    {
        var result = await countryService.GetCountryByIdAsync(countryId, ct);
        return FromResult(result);
    }

    [HttpGet("{countryId:guid}/counties", Name = "ListAllCountryCountiesAsync")]
    [ProducesResponseType(typeof(PagedResult<CountyDto>), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [EndpointSummary("Get a paged list of counties of a country")]
    [EndpointDescription("Retrieves a paged list of counties linked to a country, according to the passed pageNumber and pageSize.")]
    public async Task<ActionResult<PagedResult<CountyDto>>> ListAllCountryCountiesAsync([FromQuery] PageRequest pageRequest, Guid countryId, CancellationToken ct)
    {
        var result = await countyService.ListAllCountryCountiesAsync(pageRequest, countryId, ct);
        return FromResult(result);
    }
}

