using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Shala.Application.Common;
using Shala.Infrastructure.Data;
using Shala.Shared.Common;

namespace Shala.Infrastructure.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly AppDbContext _db;
    protected readonly DbSet<T> _table;

    public GenericRepository(AppDbContext db)
    {
        _db = db;
        _table = db.Set<T>();
    }

    public virtual async Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _table.ToListAsync(cancellationToken);
    }

    public virtual async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _table.FindAsync([id], cancellationToken);
    }

    public virtual async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _table.AddAsync(entity, cancellationToken);
    }

    public virtual async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        await _table.AddRangeAsync(entities, cancellationToken);
    }

    public virtual void Update(T entity)
    {
        _table.Update(entity);
    }

    public virtual void Delete(T entity)
    {
        _table.Remove(entity);
    }

    public virtual async Task<PagedResult<T>> GetPagedAsync(
        PagedRequest request,
        Expression<Func<T, bool>>? filter = null,
        Expression<Func<T, string>>? searchSelector = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = _table.AsQueryable();

        if (filter is not null)
            query = query.Where(filter);

        if (!string.IsNullOrWhiteSpace(request.SearchText) && searchSelector is not null)
        {
            var searchText = request.SearchText.Trim().ToLower();
            query = query.Where(BuildContainsExpression(searchSelector, searchText));
        }

        query = orderBy is not null ? orderBy(query) : query;

        return await query.ToPagedResultAsync(request.PageNumber, request.PageSize);
    }

    public virtual async Task<PagedResult<T>> GetPagedAsync(
        PagedRequest request,
        Expression<Func<T, bool>>? filter,
        List<Expression<Func<T, string>>>? searchSelectors,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = _table.AsQueryable();

        if (filter is not null)
            query = query.Where(filter);

        if (!string.IsNullOrWhiteSpace(request.SearchText) &&
            searchSelectors is not null &&
            searchSelectors.Count > 0)
        {
            var searchText = request.SearchText.Trim().ToLower();
            var predicate = BuildOrContainsExpression(searchSelectors, searchText);
            query = query.Where(predicate);
        }

        query = orderBy is not null ? orderBy(query) : query;

        return await query.ToPagedResultAsync(request.PageNumber, request.PageSize);
    }

    private static Expression<Func<T, bool>> BuildContainsExpression(
        Expression<Func<T, string>> selector,
        string searchText)
    {
        var parameter = selector.Parameters[0];
        var member = selector.Body;

        var nullCheck = Expression.NotEqual(member, Expression.Constant(null, typeof(string)));

        var toLowerMethod = typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes)!;
        var containsMethod = typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) })!;

        var toLowerCall = Expression.Call(member, toLowerMethod);
        var containsCall = Expression.Call(toLowerCall, containsMethod, Expression.Constant(searchText));

        var body = Expression.AndAlso(nullCheck, containsCall);

        return Expression.Lambda<Func<T, bool>>(body, parameter);
    }

    private static Expression<Func<T, bool>> BuildOrContainsExpression(
        List<Expression<Func<T, string>>> selectors,
        string searchText)
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        Expression? finalBody = null;

        var toLowerMethod = typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes)!;
        var containsMethod = typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) })!;

        foreach (var selector in selectors)
        {
            var replacedBody = ReplaceParameter(selector.Body, selector.Parameters[0], parameter);

            var nullCheck = Expression.NotEqual(replacedBody, Expression.Constant(null, typeof(string)));
            var toLowerCall = Expression.Call(replacedBody, toLowerMethod);
            var containsCall = Expression.Call(toLowerCall, containsMethod, Expression.Constant(searchText));
            var condition = Expression.AndAlso(nullCheck, containsCall);

            finalBody = finalBody is null ? condition : Expression.OrElse(finalBody, condition);
        }

        finalBody ??= Expression.Constant(true);

        return Expression.Lambda<Func<T, bool>>(finalBody, parameter);
    }

    private static Expression ReplaceParameter(Expression body, ParameterExpression source, ParameterExpression target)
    {
        return new ReplaceParameterVisitor(source, target).Visit(body)!;
    }

    private sealed class ReplaceParameterVisitor : ExpressionVisitor
    {
        private readonly ParameterExpression _source;
        private readonly ParameterExpression _target;

        public ReplaceParameterVisitor(ParameterExpression source, ParameterExpression target)
        {
            _source = source;
            _target = target;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return node == _source ? _target : base.VisitParameter(node);
        }
    }






    public virtual async Task<T?> FirstOrDefaultAsync(
    Expression<Func<T, bool>> predicate,
    CancellationToken cancellationToken = default)
    {
        return await _table.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public virtual async Task<T?> FirstOrDefaultAsync(
        Expression<Func<T, bool>> predicate,
        Func<IQueryable<T>, IQueryable<T>> include,
        CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = _table.AsQueryable();
        query = include(query);

        return await query.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public virtual async Task<List<T>> GetWhereAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await _table.Where(predicate).ToListAsync(cancellationToken);
    }

    public virtual async Task<List<T>> GetWhereAsync(
        Expression<Func<T, bool>> predicate,
        Func<IQueryable<T>, IQueryable<T>> include,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = _table.AsQueryable();
        query = include(query);
        query = query.Where(predicate);

        if (orderBy is not null)
            query = orderBy(query);

        return await query.ToListAsync(cancellationToken);
    }
}