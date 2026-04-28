using Microsoft.EntityFrameworkCore;
using Shala.Shared.Common;

namespace Shala.Application.Common;

public static class PagingExtensions
{
    public static async Task<PagedResult<T>> ToPagedResultAsync<T>(
        this IQueryable<T> query,
        int pageNumber,
        int pageSize)
    {
        var totalCount = await query.CountAsync();

        List<T> items;

        if (pageSize == -1) // All
        {
            items = await query.ToListAsync();
        }
        else
        {
            items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        return new PagedResult<T>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }
}