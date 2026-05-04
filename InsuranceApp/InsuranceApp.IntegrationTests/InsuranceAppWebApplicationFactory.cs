using InsuranceApp.Infrastructure.Persistence;
using InsuranceApp.IntegrationTests.Authentication;
using InsuranceApp.WebApi;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace InsuranceApp.IntegrationTests;

public class InsuranceAppWebApplicationFactory(string connectionString) : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((ctx, config) =>
        {
            var newConfiguration = new Dictionary<string, string?>
            {
                ["ConnectionStrings__Default"] = connectionString
            };

            config.AddInMemoryCollection(newConfiguration);
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<InsuranceAppContext>));
            services.AddDbContext<InsuranceAppContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = TestAuthHandler.Scheme;
                options.DefaultChallengeScheme = TestAuthHandler.Scheme;
                options.DefaultScheme = TestAuthHandler.Scheme;
            })
            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                TestAuthHandler.Scheme, _ => { });
        });
    }
}