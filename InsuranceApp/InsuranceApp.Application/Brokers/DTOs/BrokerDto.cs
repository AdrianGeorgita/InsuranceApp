using InsuranceApp.Domain.Enums;

namespace InsuranceApp.Application.Brokers.DTOs;

public class BrokerDto
{
    public Guid Id { get; set; }

    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public BrokerStatus Status { get; set; }

    public decimal? CommissionPercentage { get; set; }
}