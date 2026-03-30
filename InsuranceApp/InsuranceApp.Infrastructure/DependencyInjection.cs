using InsuranceApp.Application.Common.Audit;
using InsuranceApp.Application.Common.Messaging;
using InsuranceApp.Application.Common.Persistence;
using InsuranceApp.Application.Common.Repository;
using InsuranceApp.Application.Common.Validation;
using InsuranceApp.Infrastructure.Jobs;
using InsuranceApp.Infrastructure.Messaging.Auditing;
using InsuranceApp.Infrastructure.Messaging.Reporting;
using InsuranceApp.Infrastructure.Persistence;
using InsuranceApp.Infrastructure.Persistence.Repository;
using InsuranceApp.Infrastructure.Persistence.Repository.Reports;
using InsuranceApp.Infrastructure.Persistence.Repository.Reports.Strategies;
using InsuranceApp.Infrastructure.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InsuranceApp.Infrastructure;

internal static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<InsuranceAppContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("Default"));
        });

        services.AddScoped<DbContext>(sp => sp.GetRequiredService<InsuranceAppContext>());
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<InsuranceAppContext>());

        services.RegisterHealthChecks();

        services.AddScoped<IRequestValidator, RequestValidator>();

        services.AddStrategies();
        services.AddRepositories();
        services.AddMessaging();
        services.AddServices();

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
        services.AddScoped<ICountyRepository, CountyRepository>();
        services.AddScoped<ICityRepository, CityRepository>();
        services.AddScoped<IClientRepository, ClientRepository>();
        services.AddScoped<IBuildingRepository, BuildingRepository>();
        services.AddScoped<IRiskIndicatorRepository, RiskIndicatorRepository>();
        services.AddScoped<IBrokerRepository, BrokerRepository>();
        services.AddScoped<ICurrencyRepository, CurrencyRepository>();
        services.AddScoped<IRiskFactorConfigurationRepository, RiskFactorConfigurationRepository>();
        services.AddScoped<IFeeConfigurationRepository, FeeConfigurationRepository>();
        services.AddScoped<IPolicyRepository, PolicyRepository>();
        services.AddScoped<IReportRepository, ReportRepository>();

        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<PolicyExpiryJob, PolicyExpiryJob>();
        return services;
    }

    private static IServiceCollection AddStrategies(this IServiceCollection services)
    {
        services.AddReportGroupingStrategies();

        return services;
    }

    private static IServiceCollection AddReportGroupingStrategies(this IServiceCollection services)
    {
        services.AddScoped<IReportStrategy, CountryReportGroupingStrategy>();
        services.AddScoped<IReportStrategy, CountyReportGroupingStrategy>();
        services.AddScoped<IReportStrategy, CityReportGroupingStrategy>();
        services.AddScoped<IReportStrategy, BrokerReportGroupingStrategy>();

        return services;
    }

    private static IServiceCollection AddMessaging(this IServiceCollection services)
    {
        services.AddSingleton<IPolicyEventQueue, PolicyEventQueue>();
        services.AddScoped<IPolicyEventPublisher, PolicyEventPublisher>();
        services.AddHostedService<PolicyReplicaUpdaterBackgroundService>();
        services.AddSingleton<IAuditEventQueue, AuditEventQueue>();
        services.AddScoped<IAuditEventPublisher, AuditEventPublisher>();
        services.AddHostedService<AuditSubscriberBackgroundService>();

        return services;
    }

    private static IServiceCollection RegisterHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddDbContextCheck<InsuranceAppContext>(tags: ["ready"]);

        return services;
    }
}