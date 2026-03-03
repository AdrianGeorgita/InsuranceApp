namespace InsuranceApp.Application.Geography.DTOs;

public class CountyDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;
    public Guid CountryId { get; set; }
}