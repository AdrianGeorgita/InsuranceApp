using InsuranceApp.Application.Geography.DTOs;

namespace InsuranceApp.Application.Buildings.DTOs;

public class BuildingDto
{
    public Guid Id { get; set; }

    public Guid OwnerId { get; set; }

    public string Address { get; set; } = null!;

    public int ConstructionYear { get; set; }

    public string BuildingType { get; set; } = null!;

    public int NumberOfFloors { get; set; }

    public decimal SurfaceArea { get; set; }

    public decimal InsuredValue { get; set; }
    public string City { get; set; } = null!;
    public string County { get; set; } = null!;
    public string Country { get; set; } = null!;

    public virtual ICollection<RiskIndicatorDto> RiskIndicators { get; set; } = new List<RiskIndicatorDto>();
}