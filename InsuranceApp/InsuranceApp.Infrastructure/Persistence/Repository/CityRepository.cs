using AutoMapper;
using AutoMapper.QueryableExtensions;
using InsuranceApp.Application.Common.Pagination;
using InsuranceApp.Application.Common.Repository;
using InsuranceApp.Application.Geography.DTOs;
using InsuranceApp.Domain.Entities;
using InsuranceApp.Infrastructure.Utils;
using Microsoft.EntityFrameworkCore;

namespace InsuranceApp.Infrastructure.Persistence.Repository;

public class CityRepository(InsuranceAppContext db, IMapper mapper) : Repository<City, Guid>(db, mapper), ICityRepository
{
    private readonly IMapper _mapper = mapper;

    public async Task<PagedResult<CityDto>> GetAllCountyCitiesAsync(PageRequest pageRequest, Guid countyId, CancellationToken ct = default)
    {
        var totalSize = await db.Cities.Where(c => c.CountyId == countyId).CountAsync(ct);
        var items = await db.Cities
            .Where(c => c.CountyId == countyId)
            .OrderBy(c => c.CreatedAt)
            .Skip((pageRequest.PageNumber - 1) * pageRequest.PageSize)
            .Take(pageRequest.PageSize)
            .ProjectTo<CityDto>(_mapper.ConfigurationProvider)
            .AsNoTracking()
            .ToListAsync(ct);

        return PaginationHelpers.GetPagedResult(items, pageRequest, totalSize);
    }
}

