using System.Linq.Expressions;
using InsuranceApp.Application.Common.Pagination;

namespace InsuranceApp.Application.Common.Repository;

public interface IRepository<TEntity, TKey> where TEntity : class
{
    Task<TEntity?> GetAsync(TKey id, CancellationToken ct = default);
    Task<PagedResult<TEntity>> GetAllAsync(PageRequest pageRequest, CancellationToken ct = default);
    Task<PagedResult<TResult>> GetAllAsync<TResult>(PageRequest pageRequest, CancellationToken ct = default);
    Task<PagedResult<TEntity>> FindPagedAsync(PageRequest pageRequest, Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default);
    Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default);
    Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default);

    Task AddAsync(TEntity entity, CancellationToken ct = default);
    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken ct = default);

    void Remove(TEntity entity);
    void RemoveRange(IEnumerable<TEntity> entities);
}

