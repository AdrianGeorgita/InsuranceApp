using FluentResults;
using InsuranceApp.Application.Common.Pagination;
using InsuranceApp.Application.Geography.DTOs;

namespace InsuranceApp.Application.Geography.Counties;
public interface ICountyService
{
    Task<Result<PagedResult<CountyDto>>> ListAllCountryCountiesAsync(PageRequest pageRequest, Guid countryId, CancellationToken ct);
    Task<Result<CountyDto>> GetCountyByIdAsync(Guid guid, CancellationToken ct);
}

