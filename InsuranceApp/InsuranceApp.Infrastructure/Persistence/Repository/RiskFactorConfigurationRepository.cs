using System.Linq.Expressions;
using AutoMapper;
using InsuranceApp.Application.Common.Repository;
using InsuranceApp.Domain.Entities;
using InsuranceApp.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace InsuranceApp.Infrastructure.Persistence.Repository;

public class RiskFactorConfigurationRepository(InsuranceAppContext db, IMapper mapper) : Repository<RiskFactorConfiguration, Guid>(db, mapper), IRiskFactorConfigurationRepository
{
    public async Task<RiskFactorConfiguration?> GetConfigurationAsync(Expression<Func<RiskFactorConfiguration, bool>> predicate, CancellationToken ct)
    {
        return await db.RiskFactorConfigurations
            .AsNoTracking()
            .Where(predicate)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<IEnumerable<RiskFactorConfiguration>> GetActiveConfigurationsAsync(CancellationToken ct)
    {
        return await db.RiskFactorConfigurations
            .AsNoTracking()
            .Where(r => r.IsActive)
            .ToListAsync(ct);
    }

    public async Task<bool> ExistsBySameLevelAndReferenceIdAsync(RiskFactorConfigurationLevel level, Guid? referenceId, CancellationToken ct)
    {
        return await db.RiskFactorConfigurations.AnyAsync(r => r.Level == level && r.ReferenceId == referenceId, ct);
    }
}

