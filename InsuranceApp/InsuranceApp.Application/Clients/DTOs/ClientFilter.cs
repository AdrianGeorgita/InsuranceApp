namespace InsuranceApp.Application.Clients.DTOs;

public sealed record ClientFilter(
  string? Name,
  string? Identifier
);