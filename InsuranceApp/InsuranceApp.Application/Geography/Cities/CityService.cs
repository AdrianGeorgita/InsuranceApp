using AutoMapper;
using FluentResults;
using InsuranceApp.Application.Common.Errors;
using InsuranceApp.Application.Common.Pagination;
using InsuranceApp.Application.Common.Repository;
using InsuranceApp.Application.Geography.DTOs;

namespace InsuranceApp.Application.Geography.Cities;
public class CityService(ICityRepository cityRepository, ICountyRepository countyRepository, IMapper mapper) : ICityService
{
    public async Task<Result<PagedResult<CityDto>>> ListAllCountyCitiesAsync(PageRequest pageRequest, Guid countyId, CancellationToken ct)
    {
        var county = await countyRepository.GetAsync(countyId, ct);
        if (county is null)
            return Result.Fail(new NotFoundError($"County '{countyId}' not found."));

        var pagedResult = await cityRepository.GetAllCountyCitiesAsync(pageRequest, countyId, ct);
        return Result.Ok(pagedResult);
    }

    public async Task<Result<CityDto>> GetCityByIdAsync(Guid guid, CancellationToken ct)
    {
        var city = await cityRepository.GetAsync(guid, ct);

        if (city is null)
            return Result.Fail<CityDto>(new NotFoundError($"City '{guid}' not found."));

        return Result.Ok(mapper.Map<CityDto>(city));
    }
}

