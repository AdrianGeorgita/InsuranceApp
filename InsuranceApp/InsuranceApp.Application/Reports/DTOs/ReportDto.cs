using InsuranceApp.Domain.Enums;

namespace InsuranceApp.Application.Reports.DTOs;

public class ReportDto
{
    public string GroupingKey { get; set; } = null!;
    public string Currency { get; set; } = null!;
    public int PolicyCount { get; set; }
    public decimal TotalFinalPremium { get; set; }
    public decimal TotalFinalPremiumInBaseCurrency { get; set; }
}