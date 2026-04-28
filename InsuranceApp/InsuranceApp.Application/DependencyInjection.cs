using FluentValidation;
using InsuranceApp.Application.Audit.Cleanup;
using InsuranceApp.Application.Auth;
using InsuranceApp.Application.Auth.DTOs;
using InsuranceApp.Application.Auth.Validators;
using InsuranceApp.Application.Brokers;
using InsuranceApp.Application.Brokers.DTOs;
using InsuranceApp.Application.Brokers.Validators;
using InsuranceApp.Application.Buildings;
using InsuranceApp.Application.Buildings.DTOs;
using InsuranceApp.Application.Buildings.Validators;
using InsuranceApp.Application.Clients;
using InsuranceApp.Application.Clients.Buildings;
using InsuranceApp.Application.Clients.DTOs;
using InsuranceApp.Application.Clients.Validators;
using InsuranceApp.Application.Common.Mapping;
using InsuranceApp.Application.Geography.Cities;
using InsuranceApp.Application.Geography.Counties;
using InsuranceApp.Application.Geography.Countries;
using InsuranceApp.Application.Metadata.Currencies;
using InsuranceApp.Application.Metadata.Currencies.DTOs;
using InsuranceApp.Application.Metadata.Currencies.Validators;
using InsuranceApp.Application.Metadata.FeeConfigurations;
using InsuranceApp.Application.Metadata.FeeConfigurations.DTOs;
using InsuranceApp.Application.Metadata.FeeConfigurations.Validators;
using InsuranceApp.Application.Metadata.RiskFactorConfigurations;
using InsuranceApp.Application.Metadata.RiskFactorConfigurations.DTOs;
using InsuranceApp.Application.Metadata.RiskFactorConfigurations.Validators;
using InsuranceApp.Application.Policies;
using InsuranceApp.Application.Policies.DTOs;
using InsuranceApp.Application.Policies.Expiry;
using InsuranceApp.Application.Policies.Pricing;
using InsuranceApp.Application.Policies.Validators;
using InsuranceApp.Application.Reports;
using InsuranceApp.Application.Reports.DTOs;
using InsuranceApp.Application.Reports.Validators;
using InsuranceApp.Domain.Pricing;
using InsuranceApp.Domain.Pricing.ConfigurationStrategies.Fees;
using InsuranceApp.Domain.Pricing.ConfigurationStrategies.RiskFactors;
using Microsoft.Extensions.DependencyInjection;

namespace InsuranceApp.Application;

internal static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<RequestContext>();

        services.AddMappers();
        services.AddValidators();
        services.AddStrategies();
        services.AddServices();

        services.AddScoped<IPricingCalculator, PricingCalculator>();

        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<ICountryService, CountryService>();
        services.AddScoped<ICountyService, CountyService>();
        services.AddScoped<ICityService, CityService>();
        services.AddScoped<IClientService, ClientService>();
        services.AddScoped<IClientBuildingService, ClientBuildingService>();
        services.AddScoped<IBuildingService, BuildingService>();
        services.AddScoped<IBrokerService, BrokerService>();
        services.AddScoped<ICurrencyService, CurrencyService>();
        services.AddScoped<IRiskFactorConfigurationService, RiskFactorConfigurationService>();
        services.AddScoped<IFeeConfigurationService, FeeConfigurationService>();
        services.AddScoped<IPriceFetchService, PriceFetchService>();
        services.AddScoped<IPolicyService, PolicyService>();
        services.AddScoped<IReportService, ReportService>();
        services.AddScoped<IPolicyExpiryService, PolicyExpiryService>();
        services.AddScoped<IAuditCleanupService, AuditCleanupService>();
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }

    private static IServiceCollection AddValidators(this IServiceCollection services)
    {
        services.AddScoped<IValidator<CreateBuildingRequest>, CreateBuildingValidator>();
        services.AddScoped<IValidator<UpdateBuildingRequest>, UpdateBuildingValidator>();
        services.AddScoped<IValidator<CreateClientRequest>, CreateClientValidator>();
        services.AddScoped<IValidator<UpdateClientRequest>, UpdateClientValidator>();
        services.AddScoped<IValidator<CreateBrokerRequest>, CreateBrokerValidator>();
        services.AddScoped<IValidator<UpdateBrokerRequest>, UpdateBrokerValidator>();
        services.AddScoped<IValidator<CreateCurrencyRequest>, CreateCurrencyValidator>();
        services.AddScoped<IValidator<UpdateCurrencyRequest>, UpdateCurrencyValidator>();
        services.AddScoped<IValidator<CreateRiskFactorConfigurationRequest>, CreateRiskFactorConfigurationValidator>();
        services.AddScoped<IValidator<UpdateRiskFactorConfigurationRequest>, UpdateRiskFactorConfigurationValidator>();
        services.AddScoped<IValidator<CreateFeeConfigurationRequest>, CreateFeeConfigurationValidator>();
        services.AddScoped<IValidator<UpdateFeeConfigurationRequest>, UpdateFeeConfigurationValidator>();
        services.AddScoped<IValidator<CreatePolicyRequest>, CreatePolicyValidator>();
        services.AddScoped<IValidator<UpdatePolicyRequest>, UpdatePolicyValidator>();
        services.AddScoped<IValidator<ReportFilter>, ReportFilterValidator>();
        services.AddScoped<IValidator<RegisterRequest>, RegisterValidator>();
        services.AddScoped<IValidator<LoginRequest>, LoginValidator>();

        return services;
    }

    private static IServiceCollection AddMappers(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(InsuranceMapper));

        return services;
    }

    private static IServiceCollection AddStrategies(this IServiceCollection services)
    {
        services.AddRiskFactorConfigurationStrategies();
        services.AddFeeConfigurationStrategies();

        return services;
    }

    private static IServiceCollection AddRiskFactorConfigurationStrategies(this IServiceCollection services)
    {
        services.AddScoped<IRiskFactorConfigurationStrategy, CityRiskFactorConfigurationStrategy>();
        services.AddScoped<IRiskFactorConfigurationStrategy, CountyRiskFactorConfigurationStrategy>();
        services.AddScoped<IRiskFactorConfigurationStrategy, CountryRiskFactorConfigurationStrategy>();
        services.AddScoped<IRiskFactorConfigurationStrategy, BuildingTypeRiskFactorConfigurationStrategy>();

        return services;
    }

    private static IServiceCollection AddFeeConfigurationStrategies(this IServiceCollection services)
    {
        services.AddScoped<IFeeConfigurationStrategy, BrokerCommissionFeeStrategy>();
        services.AddScoped<IFeeConfigurationStrategy, AdminFeeStrategy>();
        services.AddScoped<IFeeConfigurationStrategy, RiskAdjustmentFeeStrategy>();

        return services;
    }
}