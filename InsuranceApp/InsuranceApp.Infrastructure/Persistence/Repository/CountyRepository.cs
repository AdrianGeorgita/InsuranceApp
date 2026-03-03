using AutoMapper;
using AutoMapper.QueryableExtensions;
using InsuranceApp.Application.Common.Pagination;
using InsuranceApp.Application.Common.Repository;
using InsuranceApp.Application.Geography.DTOs;
using InsuranceApp.Domain.Entities;
using InsuranceApp.Infrastructure.Utils;
using Microsoft.EntityFrameworkCore;

namespace InsuranceApp.Infrastructure.Persistence.Repository;

public class CountyRepository(InsuranceAppContext db, IMapper mapper) : Repository<County, Guid>(db, mapper), ICountyRepository
{
    private readonly IMapper _mapper = mapper;

    public async Task<PagedResult<CountyDto>> GetAllCountryCountiesAsync(PageRequest pageRequest, Guid countryId, CancellationToken ct = default)
    {
        var totalSize = await db.Counties.Where(c => c.CountryId == countryId).CountAsync(ct);
        var items = await db.Counties
            .Where(c => c.CountryId == countryId)
            .OrderBy(c => c.CreatedAt)
            .Skip((pageRequest.PageNumber - 1) * pageRequest.PageSize)
            .Take(pageRequest.PageSize)
            .ProjectTo<CountyDto>(_mapper.ConfigurationProvider)
            .AsNoTracking()
            .ToListAsync(ct);

        return PaginationHelpers.GetPagedResult(items, pageRequest, totalSize);
    }
}

