using System.ComponentModel.DataAnnotations;
using InsuranceApp.Domain.Entities;
using InsuranceApp.Domain.Enums;

namespace InsuranceApp.Application.Buildings.DTOs;

public class UpdateBuildingRequest
{
    public string? Address { get; set; }

    public Guid? CityId { get; set; }

    public int? ConstructionYear { get; set; }

    public BuildingType? BuildingType { get; set; }

    public int? NumberOfFloors { get; set; }

    public decimal? SurfaceArea { get; set; }

    public decimal? InsuredValue { get; set; }
    public ICollection<int> RiskIndicatorIds { get; set; } = new List<int>();
}