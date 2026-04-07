namespace InsuranceApp.Infrastructure.Persistence.Options;

public sealed class AuditOptions
{
    public int RetentionYears { get; set; }
    public List<string>? IgnoredFields { get; set; } = [];
    public List<string>? SensitiveFields { get; set; } = [];
}