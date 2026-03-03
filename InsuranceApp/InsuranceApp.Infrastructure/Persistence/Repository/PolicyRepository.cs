using AutoMapper;
using AutoMapper.QueryableExtensions;
using InsuranceApp.Application.Common.Pagination;
using InsuranceApp.Application.Common.Repository;
using InsuranceApp.Application.Policies.DTOs;
using InsuranceApp.Domain.Entities;
using InsuranceApp.Infrastructure.Persistence.QueryExtensions;
using InsuranceApp.Infrastructure.Utils;
using Microsoft.EntityFrameworkCore;

namespace InsuranceApp.Infrastructure.Persistence.Repository;

public class PolicyRepository(InsuranceAppContext db, IMapper mapper) : Repository<Policy, string>(db, mapper), IPolicyRepository
{
    private readonly IMapper _mapper = mapper;

    public async Task<PagedResult<PolicyDto>> GetAllPoliciesAsync(PageRequest pageRequest, PolicyFilter? filter, CancellationToken ct = default)
    {
        var totalSize = await db.Policies.ApplyFilter(filter).CountAsync(ct);
        var items = await db.Policies
            .ApplyFilter(filter)
            .OrderBy(c => c.CreatedAt)
            .Skip((pageRequest.PageNumber - 1) * pageRequest.PageSize)
            .Take(pageRequest.PageSize)
            .ProjectTo<PolicyDto>(_mapper.ConfigurationProvider)
            .AsNoTracking()
            .ToListAsync(ct);

        return PaginationHelpers.GetPagedResult(items, pageRequest, totalSize);
    }

    public async Task<bool> ExistsByPolicyNumberAsync(string policyNumber, CancellationToken ct)
    {
        return await db.Policies.AnyAsync(p => p.PolicyNumber == policyNumber, ct);
    }

    public async Task<Policy?> GetDetailedPolicyByIdAsync(string policyNumber, CancellationToken ct)
    {
        return await db.Policies
            .Where(p => p.PolicyNumber == policyNumber)
            .Include(p => p.Building)
                .ThenInclude(b => b.City)
                .ThenInclude(c => c.County)
                .ThenInclude(cty => cty.Country)
            .Include(p => p.Client)
            .Include(p => p.Broker)
            .FirstOrDefaultAsync(ct);
    }
}

