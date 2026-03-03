using InsuranceApp.Application.Buildings.DTOs;
using InsuranceApp.Application.Clients.DTOs;
using InsuranceApp.Domain.Enums;

namespace InsuranceApp.Application.Policies.DTOs;

public class DetailedPolicyDto
{
    public string PolicyNumber { get; set; } = null!;

    public PolicyStatus Status { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public decimal BasePremium { get; set; }

    public string CurrencyCode { get; set; } = null!;

    public decimal FinalPremium { get; set; }

    public ClientDto Client { get; set; } = null!;

    public BuildingDto Building { get; set; } = null!;

    public Guid BrokerId { get; set; }
}