namespace InsuranceApp.Application.Metadata.Currencies.DTOs;

public class UpdateCurrencyRequest
{
    public string? Code { get; set; }
    public string? Name { get; set; } 
    public decimal? ExchangeRateToBase { get; set; }
    public bool? IsActive { get; set; }
}