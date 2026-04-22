using System.Linq.Expressions;
using Shala.Shared.Common;

namespace Shala.Application.Common;

public interface IGenericRepository<T> where T : class
{
    Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task AddAsync(T entity, CancellationToken cancellationToken = default);

    Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    void Update(T entity);

    void Delete(T entity);

    Task<PagedResult<T>> GetPagedAsync(
        PagedRequest request,
        Expression<Func<T, bool>>? filter = null,
        Expression<Func<T, string>>? searchSelector = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        CancellationToken cancellationToken = default);

    Task<PagedResult<T>> GetPagedAsync(
        PagedRequest request,
        Expression<Func<T, bool>>? filter,
        List<Expression<Func<T, string>>>? searchSelectors,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        CancellationToken cancellationToken = default);




    Task<T?> FirstOrDefaultAsync(
    Expression<Func<T, bool>> predicate,
    CancellationToken cancellationToken = default);

    Task<T?> FirstOrDefaultAsync(
        Expression<Func<T, bool>> predicate,
        Func<IQueryable<T>, IQueryable<T>> include,
        CancellationToken cancellationToken = default);

    Task<List<T>> GetWhereAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default);

    Task<List<T>> GetWhereAsync(
        Expression<Func<T, bool>> predicate,
        Func<IQueryable<T>, IQueryable<T>> include,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        CancellationToken cancellationToken = default);
}