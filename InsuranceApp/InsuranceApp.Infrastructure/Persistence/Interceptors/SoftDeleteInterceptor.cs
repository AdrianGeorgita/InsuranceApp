using InsuranceApp.Domain.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace InsuranceApp.Infrastructure.Persistence.Interceptors;

public class SoftDeleteInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        if (eventData.Context is null)
            return result;

        foreach (var entry in eventData.Context.ChangeTracker.Entries())
        {
            if (entry is not { State: EntityState.Deleted, Entity: ISoftDeletable delete }) continue;
            
            entry.State = EntityState.Modified;
            delete.IsDeleted = true;
            delete.DeletedAt = DateTime.UtcNow;
        }

        return result;
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is null)
            return ValueTask.FromResult(result);

        foreach (var entry in eventData.Context.ChangeTracker.Entries())
        {
            if (entry is not { State: EntityState.Deleted, Entity: ISoftDeletable delete }) continue;

            entry.State = EntityState.Modified;
            delete.IsDeleted = true;
            delete.DeletedAt = DateTime.UtcNow;
        }

        return ValueTask.FromResult(result);
    }
}