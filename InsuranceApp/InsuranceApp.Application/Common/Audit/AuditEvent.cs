namespace InsuranceApp.Application.Common.Audit;

public class AuditEvent : IAuditEvent
{
    public Guid EventId { get; init; }
    public DateTime OccurredAt { get; init; }
    public Guid UserId { get; init; }
    public string RowId { get; init; } = null!;
    public string Action { get; init; } = null!;
    public string TableName { get; init; } = null!;
    public string ColumnName { get; init; } = null!;
    public string? OldValue { get; init; } = null!;
    public string? NewValue { get; init; } = null!;
}