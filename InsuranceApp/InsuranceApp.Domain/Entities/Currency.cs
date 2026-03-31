using InsuranceApp.Domain.Common.Interfaces;

namespace InsuranceApp.Domain.Entities;

public partial class Currency : IAuditable
{
    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public decimal ExchangeRateToBase { get; set; }

    public bool IsActive { get; set; }

    public bool Deprecated { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<Policy> Policies { get; set; } = new List<Policy>();
}