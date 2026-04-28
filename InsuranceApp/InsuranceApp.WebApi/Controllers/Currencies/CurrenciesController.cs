using InsuranceApp.Application.Common.Constants;
using InsuranceApp.Application.Common.Pagination;
using InsuranceApp.Application.Metadata.Currencies;
using InsuranceApp.Application.Metadata.Currencies.DTOs;
using InsuranceApp.WebApi.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InsuranceApp.WebApi.Controllers.Currencies;

[ApiController]
[Authorize(Roles = AppRoles.Admin)]
[Route("api/admin/[controller]")]
public class CurrenciesController(ICurrencyService currencyService) : BaseApiController
{
    [HttpGet("", Name = "ListAllCurrenciesAsync")]
    [ProducesResponseType(typeof(PagedResult<CurrencyDto>), 200)]
    [EndpointSummary("Get a paged list of currencies")]
    [EndpointDescription("Retrieves a paged list of currencies according to the passed pageNumber and pageSize.")]
    public async Task<ActionResult<PagedResult<CurrencyDto>>> ListAllCurrenciesAsync([FromQuery] PageRequest pageRequest,
        CancellationToken ct)
    {
        var result = await currencyService.ListAllCurrenciesAsync(pageRequest, ct);
        return FromResult(result);
    }

    [HttpGet("{currencyCode}", Name = "GetCurrencyByCodeAsync")]
    [ProducesResponseType(typeof(CurrencyDto), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [EndpointSummary("Get currency by Id")]
    [EndpointDescription("Retrieves a single currency using its unique code.")]
    public async Task<ActionResult<CurrencyDto>> GetCurrencyByCodeAsync(string currencyCode, CancellationToken ct)
    {
        var result = await currencyService.GetCurrencyByIdAsync(currencyCode, ct);
        return FromResult(result);
    }

    [HttpPost("", Name = "CreateCurrencyAsync")]
    [ProducesResponseType(typeof(string), 201)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 409)]
    [EndpointSummary("Create a new currency")]
    [EndpointDescription("Creates a new currency according to the passed currency code")]
    [UnitOfWork]
    public async Task<ActionResult<string>> CreateCurrencyAsync(CreateCurrencyRequest createCurrencyDto, CancellationToken ct)
    {
        var result = await currencyService.CreateCurrencyAsync(createCurrencyDto, ct);
        return FromCreated(result, "GetCurrencyByCodeAsync", id => new { currencyCode = id });
    }

    [HttpPatch("{currencyCode}", Name = "UpdateCurrencyAsync")]
    [ProducesResponseType(typeof(string), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 409)]
    [EndpointSummary("Update a currency")]
    [EndpointDescription("Updates a currency using its unique code.")]
    [UnitOfWork]
    public async Task<ActionResult<string>> UpdateCurrencyAsync(string currencyCode, UpdateCurrencyRequest updateCurrencyDto, CancellationToken ct)
    {
        var result = await currencyService.UpdateCurrencyAsync(currencyCode, updateCurrencyDto, ct);
        return FromResult(result);
    }
}