namespace InsuranceApp.Infrastructure.Persistence.Options;

public sealed class JwtOptions
{
    public string Issuer { get; init; } = null!;
    public string Audience { get; init; } = null!;
    public int Duration { get; init; }
    public string Key { get; init; } = null!;
}