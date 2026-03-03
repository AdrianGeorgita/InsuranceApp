using InsuranceApp.Domain.Enums;

namespace InsuranceApp.Application.Brokers.DTOs;

public class CreateBrokerRequest
{
    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public required BrokerStatus Status { get; set; }

    public decimal? CommissionPercentage { get; set; }
}