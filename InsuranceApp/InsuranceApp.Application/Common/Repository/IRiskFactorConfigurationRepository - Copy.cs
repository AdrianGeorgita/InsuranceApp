using InsuranceApp.Domain.Entities;
using System.Linq.Expressions;
using InsuranceApp.Domain.Enums;

namespace InsuranceApp.Application.Common.Repository;
public interface IRiskFactorConfigurationRepository : IRepository<RiskFactorConfiguration, Guid>
{
    Task<RiskFactorConfiguration?> GetConfigurationAsync(Expression<Func<RiskFactorConfiguration, bool>> predicate, CancellationToken ct);
    Task<IEnumerable<RiskFactorConfiguration>> GetActiveConfigurationsAsync(CancellationToken ct);
    Task<bool> ExistsBySameLevelAndReferenceIdAsync(RiskFactorConfigurationLevel level, Guid? referenceId, CancellationToken ct);
}

