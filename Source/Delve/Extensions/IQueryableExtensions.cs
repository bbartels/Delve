using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Delve.Models
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
            var test = source.Skip((pageNum - 1) * param.PageSize).Take(param.PageSize);
            var items = await ToListAsync(test);
            return new PagedResult<T>(items, pageNum, param.PageSize, count);
        }

        public static IPagedResult<T> ToPagedResult<T>(this IQueryable<T> source, IResourceParameter<T> param)
        {
            int count = source.Count();
            int pageNum = GetPageNum(count, param.PageSize, param.PageNumber);

            var items = source.Skip((pageNum - 1) * param.PageSize).Take(param.PageSize).ToList();
            return new PagedResult<T>(items, pageNum, param.PageSize, count);
        }

        private static int GetPageNum(int count, int pageSize, int pageNum)
        {
            int totalPages = (count + pageSize - 1) / pageSize;
            return pageNum > totalPages ? totalPages : pageNum;
        }

        public static IQueryable<T> ApplyFilters<T>(this IQueryable<T> source, IResourceParameter<T> param)
        {
            if (source == null) { throw new ArgumentException($"{ nameof(source) }"); }

            return ((IInternalResourceParameter<T>)param).ApplyFilters(source);
        }

        public static IQueryable<T> ApplyOrderBy<T>(this IQueryable<T> source, IResourceParameter<T> param)
        {
            if (source == null) { throw new ArgumentException($"{ nameof(source) }"); }

            
            source = ((IInternalResourceParameter<T>)param).ApplyOrderBy(source);
            return source;
        }

        public static IQueryable<T> ApplyIncludes<T>(this IQueryable<T> source, 
            Func<IQueryable<T>, string, IQueryable<T>> Include, IResourceParameter<T> param)
        {
            if (source == null) { throw new ArgumentException($"{ nameof(source) }"); }
            return Include == null ? source : ((IInternalResourceParameter<T>)param).ApplyExpand(source, Include);
        }
    }
}
