namespace InsuranceApp.Application.Common.Audit;

public interface IAuditEventPublisher
{
    Task PublishAuditEventAsync(AuditTableChangeEvent auditEvent, CancellationToken ct);
}