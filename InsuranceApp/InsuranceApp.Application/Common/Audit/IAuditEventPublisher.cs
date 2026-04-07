namespace InsuranceApp.Application.Common.Audit;

public interface IAuditEventPublisher
{
    Task PublishAuditEventAsync(IAuditEvent auditEvent, CancellationToken ct);
}