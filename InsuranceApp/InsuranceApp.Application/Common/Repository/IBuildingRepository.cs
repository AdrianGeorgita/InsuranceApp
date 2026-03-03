using InsuranceApp.Application.Buildings.DTOs;
using InsuranceApp.Application.Common.Pagination;
using InsuranceApp.Domain.Entities;

namespace InsuranceApp.Application.Common.Repository;
public interface IBuildingRepository : IRepository<Building, Guid>
{
    Task<PagedResult<BuildingDto>> GetAllClientBuildings(Guid clientId, PageRequest pageRequest, CancellationToken ct = default);
    Task<Building?> GetBuildingWithIndicatorsById(Guid buildingId, CancellationToken ct = default);
}

