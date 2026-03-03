namespace InsuranceApp.Application.Geography.DTOs;

public class CityDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;
    public Guid CountyId { get; set; }
}