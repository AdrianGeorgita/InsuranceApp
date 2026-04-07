namespace InsuranceApp.Infrastructure.Persistence.Options;

public sealed class AuditOptions
{
    public List<string>? IgnoredFields { get; set; } = [];
    public List<string>? SensitiveFields { get; set; } = [];
}