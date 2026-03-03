using InsuranceApp.Application.Common.Audit;

namespace InsuranceApp.Infrastructure.Messaging.Auditing;

public interface IAuditEventQueue
{
    ValueTask EnqueueAsync(AuditTableChangeEvent auditEvent, CancellationToken ct);

    ValueTask<AuditTableChangeEvent?> DequeueAsync(TimeSpan? timeout = null, CancellationToken ct = default);
}