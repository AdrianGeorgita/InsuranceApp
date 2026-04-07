namespace InsuranceApp.Domain.Entities;

public partial class PolicyAuditLog
{
    public Guid Id { get; set; }

    public Guid BrokerId { get; set; }

    public string ColumnName { get; set; } = null!;

    public string PolicyNumber { get; set; } = null!;

    public string? OldValue { get; set; }

    public string? NewValue { get; set; }
    public string? Reason { get; set; }

    public DateTime OccurredAt { get; set; }
}
