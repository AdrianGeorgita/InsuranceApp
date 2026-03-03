using FluentResults;
using InsuranceApp.Application.Common.Pagination;
using InsuranceApp.Application.Geography.DTOs;

namespace InsuranceApp.Application.Geography.Cities;
public interface ICityService
{
    Task<Result<PagedResult<CityDto>>> ListAllCountyCitiesAsync(PageRequest pageRequest, Guid countyId, CancellationToken ct);
    Task<Result<CityDto>> GetCityByIdAsync(Guid guid, CancellationToken ct);
}

