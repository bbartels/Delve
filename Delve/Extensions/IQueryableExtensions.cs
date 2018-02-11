using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

using Delve.Models;

namespace Delve.Extensions
{
    public static class IQueryableExtensions
    {
        public static async Task<IPagedResult<T>> ToPagedResultAsync<T>(this IQueryable<T> source, 
            Func<IQueryable<T>, Task<int>> CountAsync,
            Func<IQueryable<T>, Task<List<T>>> ToListAsync, 
            IResourceParameter param)
        {
            int count = await CountAsync(source);
            int pageNum = GetPageNum(count, param.PageSize, param.PageNumber);

            var items = await ToListAsync(source.Skip((pageNum - 1) * param.PageSize).Take(param.PageSize));
            return new Models.PagedResult<T>(items, pageNum, param.PageSize, count);
        }

        public static IPagedResult<T> ToPagedResult<T>(this IQueryable<T> source, IResourceParameter param)
        {
            int count = source.Count();
            int pageNum = GetPageNum(count, param.PageSize, param.PageNumber);

            var items = source.Skip((pageNum - 1) * param.PageSize).Take(param.PageSize).ToList();
            return new Models.PagedResult<T>(items, pageNum, param.PageSize, count);
        }

        private static int GetPageNum(int count, int pageSize, int pageNum)
        {
            int totalPages = (count + pageSize - 1) / pageSize;
            return pageNum > totalPages ? totalPages : pageNum;
        }

        public static IQueryable<T> ApplyFilters<T>(this IQueryable<T> source, IResourceParameter parameters)
        {
            if (source == null) { throw new ArgumentException($"{ nameof(source) }"); }

            var (query, values) = FilterExpression.GetDynamicLinqQuery(parameters.Filter);
            if (query == null || values == null) { return source; }

            return source.Where(query, values);
        }

        public static IQueryable<T> ApplyOrderBy<T>(this IQueryable<T> source, IResourceParameter parameters)
        {
            if (source == null) { throw new ArgumentException($"{ nameof(source) }"); }

            var query = OrderByExpression.GetDynamicLinqQuery(parameters.OrderBy);

            return query == null ? source : source.OrderBy(query);
        }
    }
}
