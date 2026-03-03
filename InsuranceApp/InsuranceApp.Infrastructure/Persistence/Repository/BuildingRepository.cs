using AutoMapper;
using AutoMapper.QueryableExtensions;
using InsuranceApp.Application.Buildings.DTOs;
using InsuranceApp.Application.Common.Pagination;
using InsuranceApp.Application.Common.Repository;
using InsuranceApp.Domain.Entities;
using InsuranceApp.Infrastructure.Utils;
using Microsoft.EntityFrameworkCore;

namespace InsuranceApp.Infrastructure.Persistence.Repository;

public class BuildingRepository(InsuranceAppContext db, IMapper mapper) : Repository<Building, Guid>(db, mapper), IBuildingRepository
{
    private readonly IMapper _mapper = mapper;
    public async Task<PagedResult<BuildingDto>> GetAllClientBuildings(Guid clientId, PageRequest pageRequest, CancellationToken ct = default)
    {
        var totalSize = await db.Buildings.Where(b => b.OwnerId == clientId).CountAsync(ct);
        var items = await db.Buildings
            .Where(b => b.OwnerId == clientId)
            .OrderBy(c => c.CreatedAt)
            .Skip((pageRequest.PageNumber - 1) * pageRequest.PageSize)
            .Take(pageRequest.PageSize)
            .Include(b => b.RiskIndicators)
            .ProjectTo<BuildingDto>(_mapper.ConfigurationProvider)
            .AsNoTracking()
            .ToListAsync(ct);

        return PaginationHelpers.GetPagedResult(items, pageRequest, totalSize);
    }

    public async Task<Building?> GetBuildingWithIndicatorsById(Guid buildingId, CancellationToken ct = default)
    {
        return await db.Buildings
            .Where(b => b.Id == buildingId)
            .Include(b => b.RiskIndicators)
            .Include(b => b.City)
                .ThenInclude(c => c.County)
                    .ThenInclude(county => county.Country)
            .FirstOrDefaultAsync(ct);
    }
}

