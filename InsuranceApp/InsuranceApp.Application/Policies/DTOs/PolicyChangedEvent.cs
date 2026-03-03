using InsuranceApp.Domain.Entities;

namespace InsuranceApp.Application.Policies.DTOs;

public record PolicyChangedEvent(Policy Policy);