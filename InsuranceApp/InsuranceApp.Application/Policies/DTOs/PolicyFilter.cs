using InsuranceApp.Domain.Enums;

namespace InsuranceApp.Application.Policies.DTOs;

public sealed record PolicyFilter(
    Guid? ClientId,
    Guid? BrokerId,
    PolicyStatus? Status,
    DateTime? StartDate,
    DateTime? EndDate
);