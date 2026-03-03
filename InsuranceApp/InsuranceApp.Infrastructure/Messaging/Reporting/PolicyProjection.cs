using InsuranceApp.Domain.Enums;

namespace InsuranceApp.Infrastructure.Messaging.Reporting;

public sealed class PolicyProjection
{
    public required string PolicyNumber { get; init; }
    public Guid BuildingId { get; init; }
    public Guid BrokerId { get; init; }
    public required string BrokerName { get; init; }
    public required string BuildingType { get; init; }
    public required string CountryName { get; init; }
    public required string CountyName { get; init; }
    public required string CityName { get; init; }
    public required PolicyStatus Status { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public required string CurrencyCode { get; init; }
    public decimal FinalPremium { get; init; }
}