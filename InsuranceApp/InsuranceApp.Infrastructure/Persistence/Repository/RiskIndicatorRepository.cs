using AutoMapper;
using InsuranceApp.Application.Common.Repository;
using InsuranceApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InsuranceApp.Infrastructure.Persistence.Repository;

public class RiskIndicatorRepository(InsuranceAppContext db, IMapper mapper) : Repository<RiskIndicator, int>(db, mapper), IRiskIndicatorRepository
{

    public async Task<IEnumerable<int>> ListAllKeysAsync(CancellationToken ct = default)
    {
        return await db.RiskIndicators.AsNoTracking().Select(ri => ri.Id).ToListAsync(ct);
    }
}

