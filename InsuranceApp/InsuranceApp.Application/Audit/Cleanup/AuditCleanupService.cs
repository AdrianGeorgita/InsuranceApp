using FluentResults;
using InsuranceApp.Application.Common.Persistence;
using InsuranceApp.Application.Common.Repository;

namespace InsuranceApp.Application.Audit.Cleanup;

public class AuditCleanupService(IAuditRepository repository, IUnitOfWork uow) : IAuditCleanupService
{
    public async Task<Result<int>> DeleteExpiredAuditLogsAsync(int retentionYears, CancellationToken ct)
    {
        var expiredLogs = await repository.GetAuditLogsPastTimeSpanAsync(retentionYears, ct);
        var deletedLogs = expiredLogs.Count();
        repository.RemoveRange(expiredLogs);
        await uow.SaveChangesAsync(ct);
        return Result.Ok(deletedLogs);
    }
}