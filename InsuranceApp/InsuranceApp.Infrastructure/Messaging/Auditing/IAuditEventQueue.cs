using InsuranceApp.Application.Common.Audit;

namespace InsuranceApp.Infrastructure.Messaging.Auditing;

public interface IAuditEventQueue
{
    ValueTask EnqueueAsync(IAuditEvent auditEvent, CancellationToken ct);

    ValueTask<IAuditEvent?> DequeueAsync(TimeSpan? timeout = null, CancellationToken ct = default);
}