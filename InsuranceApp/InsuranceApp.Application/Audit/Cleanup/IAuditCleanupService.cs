using FluentResults;

namespace InsuranceApp.Application.Audit.Cleanup;

public interface IAuditCleanupService
{
    Task<Result<int>> DeleteExpiredAuditLogsAsync(int retentionYears, CancellationToken ct);
}