using InsuranceApp.Domain.Common.Interfaces;
using InsuranceApp.Domain.Enums;

namespace InsuranceApp.Domain.Entities;

public partial class Policy : IAuditable, ISoftDeletable
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

    public virtual Broker Broker { get; set; } = null!;

    public virtual Building Building { get; set; } = null!;

    public virtual Client Client { get; set; } = null!;

    public virtual Currency CurrencyCodeNavigation { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
}
