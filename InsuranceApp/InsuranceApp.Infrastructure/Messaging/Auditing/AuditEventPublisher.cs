using InsuranceApp.Application.Common.Audit;

namespace InsuranceApp.Infrastructure.Messaging.Auditing;

public class AuditEventPublisher(IAuditEventQueue queue) : IAuditEventPublisher
{
    public async Task PublishAuditEventAsync(AuditTableChangeEvent auditEvent, CancellationToken ct) =>
        await queue.EnqueueAsync(auditEvent, ct);
}