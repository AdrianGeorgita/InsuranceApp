using InsuranceApp.Domain.Entities;

namespace InsuranceApp.Application.Common.Repository;
public interface IAuditRepository : IRepository<AuditLog, Guid>
{
    Task<IEnumerable<AuditLog>> GetAuditLogsPastTimeSpanAsync(int retentionYears, CancellationToken ct);
}

