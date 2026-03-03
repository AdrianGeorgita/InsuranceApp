using AutoMapper;
using FluentResults;
using InsuranceApp.Application.Common.Errors;
using InsuranceApp.Application.Common.Pagination;
using InsuranceApp.Application.Common.Repository;
using InsuranceApp.Application.Geography.DTOs;
using InsuranceApp.Domain.Entities;

namespace InsuranceApp.Application.Geography.Countries;
public class CountryService(IRepository<Country, Guid> countryRepository, IMapper mapper) : ICountryService
{
    public async Task<Result<PagedResult<CountryDto>>> ListAllCountriesAsync(PageRequest pageRequest, CancellationToken ct)
    {
        var pagedResult = await countryRepository.GetAllAsync<CountryDto>(pageRequest, ct);
        return Result.Ok(pagedResult);
    }

    public async Task<Result<CountryDto>> GetCountryByIdAsync(Guid guid, CancellationToken ct)
    {
        var country = await countryRepository.GetAsync(guid, ct);

        if (country is null)
            return Result.Fail(new NotFoundError($"Country '{guid}' not found."));

        return Result.Ok(mapper.Map<CountryDto>(country));
    }
}

