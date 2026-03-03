using InsuranceApp.Domain.Entities;
using InsuranceApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace InsuranceApp.Infrastructure.Messaging.Reporting;

public class PolicyReplicaUpdaterBackgroundService(IPolicyEventQueue queue, 
    IServiceScopeFactory scopeFactory, ILogger<PolicyReplicaUpdaterBackgroundService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while(!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var policyEvent = await queue.DequeueAsync(TimeSpan.FromSeconds(5), ct: stoppingToken);
                if (policyEvent is null)
                    continue;
                await ProcessEventAsync(policyEvent.Policy, stoppingToken);

            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }

    private async Task ProcessEventAsync(Policy policy, CancellationToken ct)
    {
        using var scope = scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<InsuranceAppContext>();

        var src = GetPolicyProjection(policy);
        var rateToBase = await LoadExchangeRateToBaseAsync(db, src.CurrencyCode, ct);

        await UpsertReplicaAsync(db, src, rateToBase, ct);

        logger.LogInformation("Upserted Policy Replica: {PolicyNumber}", policy.PolicyNumber);
    }

    private static Task<decimal> LoadExchangeRateToBaseAsync(
        InsuranceAppContext db, string currencyCode, CancellationToken ct)
    {
        return db.Currencies
            .AsNoTracking()
            .Where(c => c.Code == currencyCode)
            .Select(c => c.ExchangeRateToBase)
            .FirstOrDefaultAsync(ct);
    }

    private static PolicyProjection GetPolicyProjection(Policy policy) => new PolicyProjection()
    {
        PolicyNumber = policy.PolicyNumber,
        BuildingId = policy.BuildingId,
        BrokerId = policy.BrokerId,
        BrokerName = policy.Broker.Name,
        BuildingType = policy.Building.BuildingType,
        CountryName = policy.Building.City.County.Country.Name,
        CountyName = policy.Building.City.County.Name,
        CityName = policy.Building.City.Name,
        Status = policy.Status,
        StartDate = policy.StartDate,
        EndDate = policy.EndDate,
        CurrencyCode = policy.CurrencyCode,
        FinalPremium = policy.FinalPremium
    };

    private static async Task UpsertReplicaAsync(InsuranceAppContext db, PolicyProjection src, 
        decimal rateToBase, CancellationToken ct)
    {
        var replica = await db.PoliciesReplicas.FindAsync([src.PolicyNumber], ct);
        if (replica is null)
        {
            replica = new PoliciesReplica { PolicyNumber = src.PolicyNumber };
            db.PoliciesReplicas.Add(replica);
        }

        replica.BuildingId = src.BuildingId;
        replica.BrokerId = src.BrokerId;
        replica.BrokerName = src.BrokerName;
        replica.BuildingType = src.BuildingType;
        replica.CountryName = src.CountryName;
        replica.CountyName = src.CountyName;
        replica.CityName = src.CityName;
        replica.Status = src.Status;
        replica.StartDate = src.StartDate;
        replica.EndDate = src.EndDate;
        replica.FinalPremium = src.FinalPremium;
        replica.CurrencyCode = src.CurrencyCode;
        replica.FinalPremiumInBaseCurrency = src.FinalPremium * rateToBase;

        await db.SaveChangesAsync(ct);
    }
}