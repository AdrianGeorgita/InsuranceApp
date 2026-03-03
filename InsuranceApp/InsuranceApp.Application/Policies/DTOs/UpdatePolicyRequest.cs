using InsuranceApp.Domain.Enums;

namespace InsuranceApp.Application.Policies.DTOs;

public class UpdatePolicyRequest
{
    public PolicyStatus NewStatus { get; set; }
    public string? Reason { get; set; }
}