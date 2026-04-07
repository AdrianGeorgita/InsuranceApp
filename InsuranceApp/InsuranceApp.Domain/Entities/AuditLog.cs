namespace InsuranceApp.Domain.Entities;

public partial class AuditLog
{
    public Guid Id { get; set; }
    public Guid EventId { get; set; }

    public Guid UserId { get; set; }

    public string Action { get; set; } = null!;

    public string TableName { get; set; } = null!;

    public string ColumnName { get; set; } = null!;

    public string RowId { get; set; } = null!;

    public string? OldValue { get; set; }

    public string? NewValue { get; set; }

    public DateTime OccurredAt { get; set; }
}
