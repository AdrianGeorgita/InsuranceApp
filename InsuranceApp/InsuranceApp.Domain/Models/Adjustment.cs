using InsuranceApp.Domain.Enums;

namespace InsuranceApp.Domain.Models;

public class Adjustment
{
    public AdjustmentTypeEnum Type { get; set; }
    public string? SubType { get; set; }
    public decimal Percentage { get; set; }
}