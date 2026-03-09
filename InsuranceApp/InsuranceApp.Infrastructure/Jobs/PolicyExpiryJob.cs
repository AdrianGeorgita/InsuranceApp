using InsuranceApp.Application.Policies.Expiry;
using Microsoft.Extensions.Logging;

namespace InsuranceApp.Infrastructure.Jobs;

public class PolicyExpiryJob(IPolicyExpiryService policyExpiryService, ILogger<PolicyExpiryJob> logger)
{
    public async Task Run(CancellationToken ct = default)
    {
        var processingResult = await policyExpiryService.MarkExpiredPoliciesAsync(ct);
        logger.LogInformation("Marked {ExpiredPoliciesNumber} as expired on {ExpiryTime}.", processingResult.Value,
            DateTime.UtcNow);
    }
}