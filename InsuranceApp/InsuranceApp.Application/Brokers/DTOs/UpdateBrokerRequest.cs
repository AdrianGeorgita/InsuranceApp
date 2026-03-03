namespace InsuranceApp.Application.Brokers.DTOs;

public class UpdateBrokerRequest
{
    public string? Code { get; set; }

    public string? Name { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public decimal? CommissionPercentage { get; set; }
}