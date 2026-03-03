using InsuranceApp.Application.Common.Pagination;
using InsuranceApp.Application.Geography.DTOs;
using InsuranceApp.Domain.Entities;

namespace InsuranceApp.Application.Common.Repository;
public interface ICityRepository : IRepository<City, Guid>
{
    Task<PagedResult<CityDto>> GetAllCountyCitiesAsync(PageRequest pageRequest, Guid countyId, CancellationToken ct = default);
}

