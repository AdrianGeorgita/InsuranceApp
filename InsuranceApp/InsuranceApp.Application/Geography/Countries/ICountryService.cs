using FluentResults;
using InsuranceApp.Application.Common.Pagination;
using InsuranceApp.Application.Geography.DTOs;

namespace InsuranceApp.Application.Geography.Countries;
public interface ICountryService
{
    Task<Result<PagedResult<CountryDto>>> ListAllCountriesAsync(PageRequest pageRequest, CancellationToken ct);
    Task<Result<CountryDto>> GetCountryByIdAsync(Guid guid, CancellationToken ct);
}

