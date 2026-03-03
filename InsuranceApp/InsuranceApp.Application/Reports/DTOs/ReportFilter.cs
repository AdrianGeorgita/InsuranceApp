using InsuranceApp.Domain.Enums;

namespace InsuranceApp.Application.Reports.DTOs;

public sealed class ReportFilter()
{
    public DateOnly From { get; set; }
    public DateOnly To { get; set; }
    public PolicyStatus? Status { get; set; }
    public string? Currency { get; set; }
    public BuildingType? BuildingType { get; set; }
};