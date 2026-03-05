using InsuranceApp.Application.Policies.DTOs;
using InsuranceApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InsuranceApp.Infrastructure.Persistence.QueryExtensions;
public static class QueryOrderingExtension
{
    public static IQueryable<TEntity> OrderByPrimaryKey<TEntity>(this IQueryable<TEntity> query, DbContext context)
    {
        var entityType = context.Model.FindEntityType(typeof(TEntity));
        var primaryKey = (entityType?.FindPrimaryKey()) ?? throw new InvalidOperationException($"No primary key defined for {typeof(TEntity).Name}");
        var property = primaryKey.Properties[0];

        return query.OrderBy(e => EF.Property<object>(e!, property.Name));
    }
}

