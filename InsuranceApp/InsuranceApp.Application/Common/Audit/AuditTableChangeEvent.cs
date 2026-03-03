namespace InsuranceApp.Application.Common.Audit;

public class AuditTableChangeEvent
{
    public Guid EventId { get; init; }
    public Guid UserId { get; init; }
    public string TableName { get; init; } = null!;
    public string ColumnName { get; init; } = null!;
    public string RowId { get; init; } = null!;
    public string? OldValue { get; init; } = null!;
    public string? NewValue { get; init; } = null!;
    public string? Reason { get; init; } = null!;
}