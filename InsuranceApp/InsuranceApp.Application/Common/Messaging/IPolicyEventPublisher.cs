using InsuranceApp.Domain.Entities;

namespace InsuranceApp.Application.Common.Messaging;

public interface IPolicyEventPublisher
{
    Task PublishPolicyEventAsync(Policy policy, CancellationToken ct);
}