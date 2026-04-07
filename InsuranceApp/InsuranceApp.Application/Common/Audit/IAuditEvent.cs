namespace InsuranceApp.Application.Common.Audit;

public interface IAuditEvent
{
    Guid EventId { get; init; }
    DateTime OccurredAt { get; init; }
}