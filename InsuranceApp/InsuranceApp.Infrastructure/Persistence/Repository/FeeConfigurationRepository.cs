using AutoMapper;
using InsuranceApp.Application.Common.Repository;
using InsuranceApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InsuranceApp.Infrastructure.Persistence.Repository;

public class FeeConfigurationRepository(InsuranceAppContext db, IMapper mapper) : Repository<FeeConfiguration, Guid>(db, mapper), IFeeConfigurationRepository
{
    public async Task<IEnumerable<FeeConfiguration>> ListAllActiveFeeConfigurationsAsync(CancellationToken ct)
    {
        return await db.FeeConfigurations
            .AsNoTracking()
            .Where(f => f.IsActive &&
                        f.EffectiveTo == null || f.EffectiveTo >= DateTime.UtcNow)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<FeeConfiguration>> GetActiveConfigurationsAtDateTimeAsync(DateTime dateTime, CancellationToken ct)
    {
        return await db.FeeConfigurations
            .AsNoTracking()
            .Where(f => f.IsActive == true &&
                        f.EffectiveFrom <= dateTime && 
                        f.EffectiveTo > dateTime)
            .ToListAsync(ct);
    }
}

