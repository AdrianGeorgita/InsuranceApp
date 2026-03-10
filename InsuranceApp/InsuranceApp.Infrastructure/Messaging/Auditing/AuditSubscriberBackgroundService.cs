using AutoMapper;
using InsuranceApp.Application.Common.Audit;
using InsuranceApp.Domain.Entities;
using InsuranceApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace InsuranceApp.Infrastructure.Messaging.Auditing;

public class AuditSubscriberBackgroundService(IAuditEventQueue queue, IServiceScopeFactory scopeFactory,
    IMapper mapper, ILogger<AuditSubscriberBackgroundService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var auditEvent = await queue.DequeueAsync(TimeSpan.FromSeconds(5), ct: stoppingToken);
                if (auditEvent is null)
                    continue;
                await ProcessEventAsync(auditEvent, stoppingToken);

            }
            catch (DbUpdateException ex)
            {
                logger.LogError(ex, "There was an error inserting the audit entry.");
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }

    private async Task ProcessEventAsync(AuditTableChangeEvent auditEvent, CancellationToken ct)
    {
        using var scope = scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<InsuranceAppContext>();

        var auditEntry = mapper.Map<AuditTableValueChange>(auditEvent);
        auditEntry.Id = Guid.NewGuid();
        auditEntry.UpdatedAt = DateTime.UtcNow;

        await db.AuditTableValueChanges.AddAsync(auditEntry, ct);
        await db.SaveChangesAsync(ct);
    }
}