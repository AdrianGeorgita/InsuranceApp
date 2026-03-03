using AutoMapper;
using FluentResults;
using InsuranceApp.Application.Common.Errors;
using InsuranceApp.Application.Common.Pagination;
using InsuranceApp.Application.Common.Repository;
using InsuranceApp.Application.Geography.DTOs;
using InsuranceApp.Domain.Entities;

namespace InsuranceApp.Application.Geography.Counties;
public class CountyService(ICountyRepository countyRepository, IRepository<Country, Guid> countryRepository, IMapper mapper) : ICountyService
{
    public async Task<Result<PagedResult<CountyDto>>> ListAllCountryCountiesAsync(PageRequest pageRequest, Guid countryId, CancellationToken ct)
    {
        var country = await countryRepository.GetAsync(countryId, ct);
        if (country is null)
            return Result.Fail(new NotFoundError($"Country '{countryId}' not found."));

        var pagedResult = await countyRepository.GetAllCountryCountiesAsync(pageRequest, countryId, ct);
        return Result.Ok(pagedResult);
    }

    public async Task<Result<CountyDto>> GetCountyByIdAsync(Guid guid, CancellationToken ct)
    {
        var county = await countyRepository.GetAsync(guid, ct);

        if (county is null)
            return Result.Fail(new NotFoundError($"County '{guid}' not found."));

        return Result.Ok(mapper.Map<CountyDto>(county));
    }
}

