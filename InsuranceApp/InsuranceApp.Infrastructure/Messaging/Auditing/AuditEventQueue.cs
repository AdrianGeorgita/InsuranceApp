using InsuranceApp.Application.Common.Audit;
using System.Threading.Channels;

namespace InsuranceApp.Infrastructure.Messaging.Auditing;

public class AuditEventQueue : IAuditEventQueue
{
    private readonly Channel<AuditTableChangeEvent> _channel =
        Channel.CreateUnbounded<AuditTableChangeEvent>(new UnboundedChannelOptions()
        {
            SingleReader = true,
            SingleWriter = false
        });

    public ValueTask EnqueueAsync(AuditTableChangeEvent auditEvent, CancellationToken ct)
    {
        return _channel.Writer.WriteAsync(auditEvent, ct);
    }

    public async ValueTask<AuditTableChangeEvent?> DequeueAsync(TimeSpan? timeout = null, CancellationToken ct = default)
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