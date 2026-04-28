using InsuranceApp.Application.Auth;
using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;

namespace InsuranceApp.WebApi.Middleware;

internal sealed class RequestContextMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext httpContext, RequestContext context)
    {
        if (httpContext.User.Identity?.IsAuthenticated != true)
        {
            await next(httpContext);
            return;
        }

        var userIdValue = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)
                          ?? httpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub);

        if (Guid.TryParse(userIdValue, out var userId))
            context.UserId = userId;

        await next(httpContext);
    }
}