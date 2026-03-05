using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using InsuranceApp.Application.Common.Pagination;
using InsuranceApp.Application.Common.Repository;
using InsuranceApp.Infrastructure.Persistence.QueryExtensions;
using InsuranceApp.Infrastructure.Utils;
using Microsoft.EntityFrameworkCore;

namespace InsuranceApp.Infrastructure.Persistence.Repository;
public class Repository<TEntity, TKey>(DbContext context, IMapper mapper) : IRepository<TEntity, TKey> where TEntity : class 
{
    protected DbSet<TEntity> Set => context.Set<TEntity>();

    public virtual async Task<TEntity?> GetAsync(TKey id, CancellationToken ct = default)
    {
        return await Set.FindAsync([id], ct).AsTask();
    }

    public virtual async Task<PagedResult<TEntity>> GetAllAsync(PageRequest pageRequest, CancellationToken ct = default)
    {
        var totalSize = await Set.CountAsync(ct);
        var items = await Set.AsNoTracking()
            .OrderByPrimaryKey<TEntity>(context)
            .Skip((pageRequest.PageNumber-1) * pageRequest.PageSize)
            .Take(pageRequest.PageSize)
            .ToListAsync(ct);

        return PaginationHelpers.GetPagedResult(items, pageRequest, totalSize);
    }

    public virtual async Task<PagedResult<TResult>> GetAllAsync<TResult>(PageRequest pageRequest, CancellationToken ct = default)
    {
        var totalSize = await Set.CountAsync(ct);
        var items = await Set.AsNoTracking()
            .OrderByPrimaryKey<TEntity>(context)
            .Skip((pageRequest.PageNumber-1) * pageRequest.PageSize)
            .Take(pageRequest.PageSize)
            .ProjectTo<TResult>(mapper.ConfigurationProvider)
            .ToListAsync(ct);

        return PaginationHelpers.GetPagedResult(items, pageRequest, totalSize);
    }

    public virtual async Task<PagedResult<TEntity>> FindPagedAsync(
        PageRequest pageRequest, Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default)
    {
        var totalSize = await Set.Where(predicate).CountAsync(ct);
        var items = await Set.AsNoTracking()
            .Where(predicate)
            .Skip((pageRequest.PageNumber - 1) * pageRequest.PageSize)
            .Take(pageRequest.PageSize)
            .ToListAsync(ct);

        return PaginationHelpers.GetPagedResult(items, pageRequest, totalSize);
    }

    public virtual async Task<IEnumerable<TEntity>> FindAsync(
        Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default)
    {
        return await Set.AsNoTracking()
            .Where(predicate)
            .ToListAsync(ct);
    }

    public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default)
    {
        return await Set.AsNoTracking().AnyAsync(predicate, ct);
    }

    public virtual async Task AddAsync(TEntity entity, CancellationToken ct = default)
    {
        await Set.AddAsync(entity, ct);
    }

    public virtual async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken ct = default)
    {
        await Set.AddRangeAsync(entities, ct);
    }

    public virtual void Remove(TEntity entity)
    {
        Set.Remove(entity);
    }

    public virtual void RemoveRange(IEnumerable<TEntity> entities)
    {
        Set.RemoveRange(entities);
    }
}

