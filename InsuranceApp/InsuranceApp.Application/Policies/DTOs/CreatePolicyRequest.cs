namespace InsuranceApp.Application.Policies.DTOs;

public class CreatePolicyRequest
{
    public Guid ClientId { get; set; }

    public Guid BuildingId { get; set; }
    public Guid BrokerId { get; set; } // temporary

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public decimal BasePremium { get; set; }

    public string CurrencyCode { get; set; } = null!;
}