using InsuranceApp.Application.Common.Messaging;
using InsuranceApp.Application.Policies.DTOs;
using InsuranceApp.Domain.Entities;

namespace InsuranceApp.Infrastructure.Messaging.Reporting;

public class PolicyEventPublisher(IPolicyEventQueue queue) : IPolicyEventPublisher
{
    public async Task PublishPolicyEventAsync(Policy policy, CancellationToken ct) => 
        await queue.EnqueueAsync(new PolicyChangedEvent(policy), ct);
}