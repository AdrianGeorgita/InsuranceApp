using InsuranceApp.Application.Common.Audit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Collections.Concurrent;
using InsuranceApp.Application.Auth;
using InsuranceApp.Domain.Common.Interfaces;
using InsuranceApp.Infrastructure.Persistence.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace InsuranceApp.Infrastructure.Persistence.Interceptors;

public class AuditInterceptor(IAuditEventPublisher auditEventPublisher
    , ILogger<AuditInterceptor> logger, IOptions<AuditOptions> options,
    RequestContext requestContext) : SaveChangesInterceptor
{
    private readonly ConcurrentDictionary<Guid, List<IAuditEvent>> _pendingEvents = new();
    private readonly AuditOptions _options = options.Value;

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;
        if (context is null)
            return ValueTask.FromResult(result);

        context.ChangeTracker.DetectChanges();

        var pendingEvents = BuildAuditEvents(context);
        if (pendingEvents.Count > 0)
        {
            _pendingEvents[context.ContextId.InstanceId] = pendingEvents;
        }

        return ValueTask.FromResult(result);
    }

    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;
        if (context is null)
            return result;

        if (_pendingEvents.TryRemove(context.ContextId.InstanceId, out var events) && events.Count > 0)
        {
            try
            {
                foreach (var auditEvent in events)
                {
                    await auditEventPublisher.PublishAuditEventAsync(auditEvent, cancellationToken);
                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to publish audit events!");
            }
        }

        return result;
    }

    public override Task SaveChangesFailedAsync(
        DbContextErrorEventData eventData,
        CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;
        if (context is not null)
        {
            _pendingEvents.TryRemove(context.ContextId.InstanceId, out _);
        }

        return Task.CompletedTask;
    }

    private List<IAuditEvent> BuildAuditEvents(DbContext context)
    {
        var occurredAt = DateTime.UtcNow;
        var auditEvents = new List<IAuditEvent>();
        var eventId = Guid.NewGuid();

        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (!ShouldAudit(entry))
                continue;

            auditEvents.AddRange(CreateEventsForEntry(entry, eventId, occurredAt));
        }

        return auditEvents;
    }


    private static bool ShouldAudit(EntityEntry entry)
    {
        if (entry.State is not (EntityState.Added or EntityState.Modified or EntityState.Deleted))
            return false;

        return entry.Entity is IAuditable;
    }

    private IEnumerable<IAuditEvent> CreateEventsForEntry(
        EntityEntry entry,
        Guid eventId,
        DateTime occurredAt)
    {
        var rowId = GetRowId(entry);
        var tableName = GetTableName(entry);
        var action = GetAction(entry.State);

        foreach (var property in entry.Properties)
        {
            if (ShouldSkipProperty(property))
                continue;

            if (entry.State == EntityState.Modified && !property.IsModified)
                continue;

            var oldValue = GetOldValue(entry.State, property);
            var newValue = GetNewValue(entry.State, property);

            if (entry.State == EntityState.Modified && Equals(oldValue, newValue))
                continue;

            yield return CreateAuditEvent(
                eventId,
                rowId,
                tableName,
                occurredAt,
                action,
                property.Metadata.Name,
                oldValue,
                newValue);
        }
    }

    private bool ShouldSkipProperty(PropertyEntry property)
    {
        var propertyName = property.Metadata.Name;

        if (_options.IgnoredFields?.Contains(propertyName) == true)
            return true;

        return false;
    }

    private string? GetOldValue(EntityState state, PropertyEntry property)
    {
        return state switch
        {
            EntityState.Added => null,
            EntityState.Modified => FormatValue(property.Metadata.Name, property.OriginalValue),
            EntityState.Deleted => FormatValue(property.Metadata.Name, property.OriginalValue),
            _ => null
        };
    }

    private string? GetNewValue(EntityState state, PropertyEntry property)
    {
        return state switch
        {
            EntityState.Added => FormatValue(property.Metadata.Name, property.CurrentValue),
            EntityState.Modified => FormatValue(property.Metadata.Name, property.CurrentValue),
            EntityState.Deleted => null,
            _ => null
        };
    }

    private string? FormatValue(string propertyName, object? value) => 
        _options.SensitiveFields?.Contains(propertyName) == true ? "**REDACTED**" : value?.ToString();

    private AuditEvent CreateAuditEvent(
        Guid eventId,
        string rowId,
        string tableName,
        DateTime occurredAt,
        string action,
        string columnName,
        object? oldValue,
        object? newValue)
    {
        return new AuditEvent
        {
            EventId = eventId,
            RowId = rowId,
            TableName = tableName,
            OccurredAt = occurredAt,
            UserId = requestContext.UserId,
            Action = action,
            ColumnName = columnName,
            OldValue = oldValue?.ToString(),
            NewValue = newValue?.ToString()
        };
    }

    private static string GetAction(EntityState state) => state.ToString();

    private static string GetTableName(EntityEntry entry) =>
        entry.Metadata.GetTableName() ?? entry.Metadata.ClrType.Name;

    private static string GetRowId(EntityEntry entry)
    {
        var primaryKey = entry.Metadata.FindPrimaryKey();
        if (primaryKey is null || primaryKey.Properties.Count == 0)
            return string.Empty;

        var keyName = primaryKey.Properties[0].Name;
        var value = entry.Property(keyName).CurrentValue
                    ?? entry.Property(keyName).OriginalValue;

        return value?.ToString() ?? string.Empty;
    }
}
