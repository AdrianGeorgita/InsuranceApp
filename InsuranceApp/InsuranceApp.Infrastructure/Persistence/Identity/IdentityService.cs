using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using FluentResults;
using InsuranceApp.Application.Auth;
using InsuranceApp.Application.Auth.DTOs;
using InsuranceApp.Application.Common.Errors;
using InsuranceApp.Infrastructure.Persistence.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace InsuranceApp.Infrastructure.Persistence.Identity;

public class IdentityService(UserManager<User> userManager, IOptions<JwtOptions> options) : IIdentityService
{
    public async Task<Result<Guid>> RegisterAsync(RegisterRequest request, CancellationToken ct)
    {
        var user = new User
        {
            UserName = request.Username,
            Email = request.Email,
        };
        var result = await userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
            return Result.Fail(result.Errors.Select(e => new ValidationError(e.Description)));

        var roleResult = await userManager.AddToRoleAsync(user, request.Role);

        if (!roleResult.Succeeded)
        {
            await userManager.DeleteAsync(user);
            return Result.Fail(roleResult.Errors.Select(e => new ValidationError(e.Description)));
        }

        user = await userManager.FindByEmailAsync(request.Email);

        return Result.Ok(user!.Id);
    }

    public async Task<Result<string>> LoginAsync(LoginRequest request, CancellationToken ct)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null)
            return Result.Fail(new NotFoundError("User not found!"));

        var isPasswordValid = await userManager.CheckPasswordAsync(user, request.Password);
        if (!isPasswordValid)
            return Result.Fail(new ValidationError("Invalid credentials!"));

        var token = await GenerateToken(user);

        return Result.Ok(token);
    }

    private async Task<string> GenerateToken(User user)
    {
        var jwtOptions = options.Value;
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var roles = await userManager.GetRolesAsync(user);
        var roleClaims = roles.Select(q => new Claim(ClaimTypes.Role, q)).ToList();

        //var userClaims = await userManager.GetClaimsAsync(user);

        var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!)
            }
            //.Union(userClaims)
            .Union(roleClaims);

        var token = new JwtSecurityToken(
            issuer: jwtOptions.Issuer,
            audience: jwtOptions.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(Convert.ToInt32(jwtOptions.Duration)),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}