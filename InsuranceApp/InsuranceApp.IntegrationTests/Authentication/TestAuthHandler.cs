using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using ILoggerFactory = Microsoft.Extensions.Logging.ILoggerFactory;

namespace InsuranceApp.IntegrationTests.Authentication;

public class TestAuthHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    public new const string Scheme = "TestScheme";

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (Request.Headers.TryGetValue(TestAuthHeaders.Anonymous, out var anonymous) &&
            anonymous == "true")
            return Task.FromResult(AuthenticateResult.NoResult());

        var userId = Request.Headers.TryGetValue(TestAuthHeaders.UserId, out var id)
            ? id.ToString()
            : "test-user-id";

        var userName = Request.Headers.TryGetValue(TestAuthHeaders.UserName, out var name)
            ? name.ToString()
            : "test-user-name";

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId),
            new(ClaimTypes.Name, userName)
        };

        if (Request.Headers.TryGetValue(TestAuthHeaders.Roles, out var rolesHeader))
        {
            var roles = rolesHeader.ToString()
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        }

        var identity = new ClaimsIdentity(claims, Scheme);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}