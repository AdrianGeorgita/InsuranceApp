using DotNetEnv;
using Hangfire;
using InsuranceApp.Application;
using InsuranceApp.Infrastructure;
using InsuranceApp.WebApi.ExceptionHandling;
using InsuranceApp.WebApi.Extensions;
using InsuranceApp.WebApi.Filters;
using Serilog;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;

namespace InsuranceApp.WebApi;

public class Program
{
    public static async Task Main(string[] args)
    {
        Env.TraversePath().Load();

        var builder = WebApplication.CreateBuilder(args);

        var connectionString = builder.Configuration.GetConnectionString("Default")
                               ?? builder.Configuration["ConnectionStrings__Default"];

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException(
                "Missing connection string. Set ConnectionStrings__Default in environment or .env");


        DbMigrator.DbMigrator.Migrate(connectionString);

        // Add services to the container.
        builder.Services.AddScoped<UnitOfWorkFilter>();
        builder.Services.AddControllers(options =>
            {
                options.Filters.AddService<UnitOfWorkFilter>();
            })
            .AddJsonOptions(o =>
            {
                o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.ConfigureSwaggerGen();

        builder.Services.AddApplication();
        builder.Services.AddInfrastructure(builder.Configuration);

        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddProblemDetails();

        builder.Host.UseSerilog((context, services, configuration) =>
        {
            configuration.ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext();   
        });

        builder.Services.AddRouting(o => o.LowercaseUrls = true);

        builder.Services.AddRateLimiter(options =>
        {
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>((httpContext) =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.Request.Headers.Host.ToString(),
                    factory: partition => new FixedWindowRateLimiterOptions()
                    {
                        AutoReplenishment = true,
                        PermitLimit = 100,
                        QueueLimit = 0,
                        Window = TimeSpan.FromSeconds(10)
                    }
                )
            );
        });

        builder.Services.AddHangfire(config =>
        {
            config.UseSqlServerStorage(builder.Configuration.GetConnectionString("Default"));
        });
        builder.Services.AddHangfireServer();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseHangfireDashboard();
        }

        app.UseSerilogRequestLogging();

        app.UseExceptionHandler();

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseRateLimiter();

        app.MapControllers();

        app.AddBackgroundJobs();

        app.MapHealthCheckEndpoints();

        await app.RunAsync();
    }
}