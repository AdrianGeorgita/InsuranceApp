using InsuranceApp.Application.Policies.DTOs;

namespace InsuranceApp.Infrastructure.Messaging.Reporting;

public interface IPolicyEventQueue
{
    ValueTask EnqueueAsync(PolicyChangedEvent policyEvent, CancellationToken ct);

    ValueTask<PolicyChangedEvent?> DequeueAsync(TimeSpan? timeout = null, CancellationToken ct = default);
}