using InsuranceApp.Application.Audit.Cleanup;
using Microsoft.Extensions.Logging;

namespace InsuranceApp.Infrastructure.Jobs;

public class AuditCleanupJob(IAuditCleanupService auditCleanupService, ILogger<AuditCleanupJob> logger)
{
    public async Task Run(int retentionYears, CancellationToken ct = default)
    {
        var processingResult = await auditCleanupService.DeleteExpiredAuditLogsAsync(retentionYears, ct);
        logger.LogInformation("Deleted {DeletedLogs} audit logs on {DeletionTime}.", processingResult.Value,
            DateTime.UtcNow);
    }
}