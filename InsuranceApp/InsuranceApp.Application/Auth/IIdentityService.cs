using FluentResults;
using InsuranceApp.Application.Auth.DTOs;

namespace InsuranceApp.Application.Auth;

public interface IIdentityService
{
    Task<Result<Guid>> RegisterAsync(RegisterRequest request, CancellationToken ct);
    Task<Result<string>> LoginAsync(LoginRequest request, CancellationToken ct);
}