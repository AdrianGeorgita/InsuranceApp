using Hangfire;
using InsuranceApp.Infrastructure.Jobs;

namespace InsuranceApp.WebApi.Extensions;

public static class BackgroundJobsExtensions
{
    public static void AddBackgroundJobs(this IHost app)
    {
        var recurringJobs = app.Services.GetRequiredService<IRecurringJobManager>();

        recurringJobs.AddOrUpdate<PolicyExpiryJob>(
            recurringJobId: "policy-expiry-job",
            methodCall: job => job.Run(CancellationToken.None),
            cronExpression: Cron.Daily
        );
    }
}