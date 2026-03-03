using InsuranceApp.Infrastructure.Persistence;
using InsuranceApp.WebApi.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace InsuranceApp.WebApi.Filters;

public class UnitOfWorkFilter(InsuranceAppContext db, ILogger<UnitOfWorkFilter> logger) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var hasAttribute = context.ActionDescriptor.EndpointMetadata.OfType<UnitOfWorkAttribute>().Any();
        if (!hasAttribute)
        {
            await next();
            return;
        }

        var cancellationToken = context.HttpContext.RequestAborted;
        await using var dbContextTransaction = await db.Database.BeginTransactionAsync(cancellationToken);

        var executed = await next();

        if ((executed.Exception is not null && !executed.ExceptionHandled)
            || executed.Result is ObjectResult { Value: ProblemDetails or ValidationProblemDetails })
        {
            await dbContextTransaction.RollbackAsync(cancellationToken);
            return;
        }

        try
        {
            await db.SaveChangesAsync(cancellationToken);
            await dbContextTransaction.CommitAsync(cancellationToken);
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, "Failed to save database changes.");
            await dbContextTransaction.RollbackAsync(cancellationToken);
        }
        catch (OperationCanceledException ex)
        {
            logger.LogError(ex, "Failed to save database changes due to cancelled operation.");
            await dbContextTransaction.RollbackAsync(cancellationToken);
        }
    }
}
