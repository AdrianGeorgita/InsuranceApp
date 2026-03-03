using InsuranceApp.Domain.Enums;

namespace InsuranceApp.Domain.Entities;

public partial class Broker
{
    public Guid Id { get; set; }

    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public BrokerStatus Status { get; set; }

    public decimal? CommissionPercentage { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<Policy> Policies { get; set; } = new List<Policy>();
}