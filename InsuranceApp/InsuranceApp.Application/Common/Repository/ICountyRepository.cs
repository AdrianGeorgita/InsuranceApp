using InsuranceApp.Application.Common.Pagination;
using InsuranceApp.Application.Geography.DTOs;
using InsuranceApp.Domain.Entities;

namespace InsuranceApp.Application.Common.Repository;
public interface ICountyRepository : IRepository<County, Guid>
{
    Task<PagedResult<CountyDto>> GetAllCountryCountiesAsync(PageRequest pageRequest, Guid countryId, CancellationToken ct = default);
}

