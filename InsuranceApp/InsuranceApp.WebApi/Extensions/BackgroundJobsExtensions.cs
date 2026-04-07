using Hangfire;
using InsuranceApp.Infrastructure.Jobs;
using InsuranceApp.Infrastructure.Persistence.Options;
using Microsoft.Extensions.Options;

namespace InsuranceApp.WebApi.Extensions;

public static class BackgroundJobsExtensions
{
    public static void AddBackgroundJobs(this IHost app)
    {
        var recurringJobs = app.Services.GetRequiredService<IRecurringJobManager>();
        var auditOptions = app.Services.GetRequiredService<IOptions<AuditOptions>>().Value;

        recurringJobs.AddOrUpdate<PolicyExpiryJob>(
            recurringJobId: "policy-expiry-job",
            methodCall: job => job.Run(CancellationToken.None),
            cronExpression: Cron.Daily
        );

        recurringJobs.AddOrUpdate<AuditCleanupJob>(
            recurringJobId: "audit-cleanup-job",
            methodCall: job => job.Run(auditOptions.RetentionYears, CancellationToken.None),
            cronExpression: Cron.Daily
        );
    }
}