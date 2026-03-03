using InsuranceApp.Domain.Enums;

namespace InsuranceApp.Domain.Entities;

public partial class PoliciesReplica
{
    public string PolicyNumber { get; set; } = null!;

    public Guid BuildingId { get; set; }

    public Guid BrokerId { get; set; }

    public string BrokerName { get; set; } = null!;

    public string BuildingType { get; set; } = null!;

    public string CountryName { get; set; } = null!;

    public string CountyName { get; set; } = null!;

    public string CityName { get; set; } = null!;

    public PolicyStatus Status { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public string CurrencyCode { get; set; } = null!;

    public decimal FinalPremium { get; set; }

    public decimal FinalPremiumInBaseCurrency { get; set; }
}
