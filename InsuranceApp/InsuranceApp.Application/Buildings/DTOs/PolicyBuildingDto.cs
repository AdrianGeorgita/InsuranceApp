using InsuranceApp.Domain.Enums;

namespace InsuranceApp.Application.Buildings.DTOs;

public class PolicyBuildingDto
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public BuildingType BuildingType { get; set; }
    public Guid CityId { get; set; }
    public Guid CountyId { get; set; }
    public Guid CountryId { get; set; }

    public virtual ICollection<RiskIndicatorDto> RiskIndicators { get; set; } = new List<RiskIndicatorDto>();
}