namespace InsuranceApp.Domain.Entities;

public partial class Building
{
    public Guid Id { get; set; }

    public Guid OwnerId { get; set; }

    public string Address { get; set; } = null!;

    public Guid CityId { get; set; }

    public int ConstructionYear { get; set; }

    public string BuildingType { get; set; } = null!;

    public int NumberOfFloors { get; set; }

    public decimal SurfaceArea { get; set; }

    public decimal InsuredValue { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual City City { get; set; } = null!;

    public virtual Client Owner { get; set; } = null!;

    public virtual ICollection<Policy> Policies { get; set; } = new List<Policy>();

    public virtual ICollection<RiskIndicator> RiskIndicators { get; set; } = new List<RiskIndicator>();
}
