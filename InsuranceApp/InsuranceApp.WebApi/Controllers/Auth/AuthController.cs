using InsuranceApp.Application.Auth;
using InsuranceApp.Application.Auth.DTOs;
using InsuranceApp.WebApi.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace InsuranceApp.WebApi.Controllers.Auth;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService) : BaseApiController
{

    [HttpPost("register", Name = "RegisterAsync")]
    [ProducesResponseType(typeof(Guid), 201)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 409)]
    [EndpointSummary("Register a new user")]
    [EndpointDescription("Creates a new user, assigns it an unique identifier and the corresponding role")]
    [UnitOfWork]
    public async Task<ActionResult<Guid>> RegisterAsync(RegisterRequest registerDto, CancellationToken ct)
    {
        var result = await authService.RegisterAsync(registerDto, ct);
        return FromResult(result);
    }

    [HttpPost("login", Name = "LoginAsync")]
    [ProducesResponseType(typeof(string), 201)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [EndpointSummary("Log in as an user")]
    [EndpointDescription("Log in as an user and receive the jwt bearer")]
    [UnitOfWork]
    public async Task<ActionResult<string>> LoginAsync(LoginRequest loginDto, CancellationToken ct)
    {
        var result = await authService.LoginAsync(loginDto, ct);
        return FromResult(result);
    }
}