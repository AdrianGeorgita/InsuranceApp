using FluentResults;
using InsuranceApp.Application.Auth.DTOs;

namespace InsuranceApp.Application.Auth;

public interface IAuthService
{
    Task<Result<Guid>> RegisterAsync(RegisterRequest registerDto, CancellationToken ct);
    Task<Result<string>> LoginAsync(LoginRequest loginDto, CancellationToken ct);
}