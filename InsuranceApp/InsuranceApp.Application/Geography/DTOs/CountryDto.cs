namespace InsuranceApp.Application.Geography.DTOs;

public class CountryDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Iso2 { get; set; } = null!;

    public string Iso3 { get; set; } = null!;
}