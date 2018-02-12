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
            IResourceParameter<T> param)
        {
            int count = await CountAsync(source);
            int pageNum = GetPageNum(count, param.PageSize, param.PageNumber);

            var items = await ToListAsync(source.Skip((pageNum - 1) * param.PageSize).Take(param.PageSize));
            return new Models.PagedResult<T>(items, pageNum, param.PageSize, count);
        }

        public static IPagedResult<T> ToPagedResult<T>(this IQueryable<T> source, IResourceParameter<T> param)
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

        public static IQueryable<T> ApplyFilters<T>(this IQueryable<T> source, IResourceParameter<T> param)
        {
            if (source == null) { throw new ArgumentException($"{ nameof(source) }"); }

            var (query, values) = FilterExpression.GetDynamicLinqQuery(((IInternalResourceParameter)param).Filter);
            if (query == null || values == null) { return source; }

            return source.Where(query, values);
        }

        public static IQueryable<T> ApplyOrderBy<T>(this IQueryable<T> source, IResourceParameter<T> param)
        {
            if (source == null) { throw new ArgumentException($"{ nameof(source) }"); }

            var query = OrderByExpression.GetDynamicLinqQuery(((IInternalResourceParameter)param).OrderBy);
            return query == null ? source : source.OrderBy(query);
        }

        public static IQueryable<T> ApplyIncludes<T>(this IQueryable<T> source, Func<IQueryable<T>, string, IQueryable<T>> Include, IResourceParameter<T> param)
        {
            if (source == null) { throw new ArgumentException($"{ nameof(source) }"); }
            if (Include == null) { return source; }

            var expands = ((IInternalResourceParameter)param).Expand;

            return expands.Aggregate(source, (current, expand) => Include(current, expand.GetDynamicLinqQuery()));
        }
    }
}
