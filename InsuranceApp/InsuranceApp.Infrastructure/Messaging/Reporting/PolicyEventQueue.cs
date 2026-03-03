using System.Threading.Channels;
using InsuranceApp.Application.Policies.DTOs;

namespace InsuranceApp.Infrastructure.Messaging.Reporting;

public class PolicyEventQueue : IPolicyEventQueue
{
    private readonly Channel<PolicyChangedEvent> _channel =
        Channel.CreateUnbounded<PolicyChangedEvent>(new UnboundedChannelOptions()
        {
            SingleReader = true,
            SingleWriter = false
        });

    public ValueTask EnqueueAsync(PolicyChangedEvent policyEvent, CancellationToken ct)
    {
        return _channel.Writer.WriteAsync(policyEvent, ct);
    }

    public async ValueTask<PolicyChangedEvent?> DequeueAsync(TimeSpan? timeout = null, CancellationToken ct = default)
    {
        if (timeout is null)
            return await _channel.Reader.ReadAsync(ct);

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        cts.CancelAfter(timeout.Value);

        try
        {
            return await _channel.Reader.ReadAsync(cts.Token);
        }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested)
        {
            return null;
        }
    }
}