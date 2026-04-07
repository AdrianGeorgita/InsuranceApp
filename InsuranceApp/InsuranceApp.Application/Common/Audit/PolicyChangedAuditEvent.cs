namespace InsuranceApp.Application.Common.Audit;

public class PolicyChangedAuditEvent : IAuditEvent
{
    public Guid EventId { get; init; }
    public DateTime OccurredAt { get; init; }
    public Guid BrokerId { get; init; }
    public string PolicyNumber { get; init; } = null!;
    public string ColumnName { get; init; } = null!;
    public string? OldValue { get; init; } = null!;
    public string? NewValue { get; init; } = null!;
    public string? Reason { get; init; } = null!;
}