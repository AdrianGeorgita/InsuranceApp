using FluentResults;
using InsuranceApp.Application.Auth.DTOs;
using InsuranceApp.Application.Common.Validation;

namespace InsuranceApp.Application.Auth;

public class AuthService(IIdentityService identityService, IRequestValidator requestValidator) : IAuthService
{
    public async Task<Result<Guid>> RegisterAsync(RegisterRequest registerDto, CancellationToken ct)
    {
        var validation = await requestValidator.ValidateAsync(registerDto, ct);
        if (!validation.IsValid)
            return Result.Fail(validation.ToApiError());

        var result = await identityService.RegisterAsync(registerDto, ct);
        return result;
    }

    public async Task<Result<string>> LoginAsync(LoginRequest loginDto, CancellationToken ct)
    {
        var validation = await requestValidator.ValidateAsync(loginDto, ct);
        if (!validation.IsValid)
            return Result.Fail(validation.ToApiError());

        var result = await identityService.LoginAsync(loginDto, ct);
        return result;
    }
}