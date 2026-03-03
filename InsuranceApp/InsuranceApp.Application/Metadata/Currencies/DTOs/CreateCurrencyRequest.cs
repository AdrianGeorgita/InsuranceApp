namespace InsuranceApp.Application.Metadata.Currencies.DTOs;

public class CreateCurrencyRequest
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public decimal ExchangeRateToBase { get; set; }
    public bool IsActive { get; set; }
}