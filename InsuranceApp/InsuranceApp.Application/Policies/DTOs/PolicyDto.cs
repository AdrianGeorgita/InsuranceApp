using InsuranceApp.Domain.Enums;

namespace InsuranceApp.Application.Policies.DTOs;

public class PolicyDto
{
    public string PolicyNumber { get; set; } = null!;

    public Guid ClientId { get; set; }

    public Guid BuildingId { get; set; }

    public Guid BrokerId { get; set; }

    public PolicyStatus Status { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public decimal BasePremium { get; set; }

    public string CurrencyCode { get; set; } = null!;

    public decimal FinalPremium { get; set; }
}