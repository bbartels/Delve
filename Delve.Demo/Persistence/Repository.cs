using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Delve.Extensions;
using Microsoft.EntityFrameworkCore;

using Delve.Models;

namespace Delve.Demo.Persistence
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected DbContext Context { get; }

        public Repository(DbContext context)
        {
            Context = context;
        }

        public T GetById(int id)
        {
            return Context.Set<T>().Find(id);
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await Context.Set<T>().FindAsync(id);
        }

        public IEnumerable<T> Get(Expression<Func<T, bool>> predicate = null, 
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, 
            string includes = null, 
            int? skip = null,
            int? take = null)
        {
            return BuildQueryable(predicate, orderBy, includes, skip, take).ToList();
        }

        public async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> predicate = null, 
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, 
            string includes = null, 
            int? skip = null,
            int? take = null)
        {
            return await BuildQueryable(predicate, orderBy, includes, skip, take).ToListAsync();
        }

        public IPagedResult<T> Get(IResourceParameter<T> parameters)
        {
            var collection = Context.Set<T>().ApplyFilters(parameters);
            return collection.ToPagedResult(parameters);
        }

        public async Task<IPagedResult<T>> GetAsync(IResourceParameter<T> parameters)
        {
            var collection = Context.Set<T>().ApplyFilters(parameters);
            return await collection.ToPagedResultAsync(async q => await q.CountAsync(), async q => await q.ToListAsync(), parameters);
        }

        public T SingleOrDefault(Expression<Func<T, bool>> predicate)
        {
            return Context.Set<T>().SingleOrDefault(predicate);
        }

        public async Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return await Context.Set<T>().SingleOrDefaultAsync(predicate);
        }

        public int GetCount(Expression<Func<T, bool>> predicate = null)
        {
            return BuildQueryable(predicate).Count();
        }

        public async Task<int> GetCountAsync(Expression<Func<T, bool>> predicate = null)
        {
            return await BuildQueryable(predicate).CountAsync();
        }

        public bool GetExists(Expression<Func<T, bool>> predicate = null)
        {
            return BuildQueryable(predicate).Any();
        }

        public async Task<bool> GetExistsAsync(Expression<Func<T, bool>> predicate = null)
        {
            return await BuildQueryable(predicate).AnyAsync();
        }

        public void Add(T entity)
        {
            Context.Set<T>().Add(entity);
        }

        public void AddRange(IEnumerable<T> entities)
        {
            Context.Set<T>().AddRange(entities);
        }

        public void Remove(T entity)
        {
            Context.Set<T>().Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            Context.Set<T>().RemoveRange(entities);
        }


        protected virtual IQueryable<T> BuildQueryable(
            Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includes = null,
            int? skip = null,
            int? take = null)
        {
            includes = includes ?? string.Empty;
            IQueryable<T> query = Context.Set<T>();

            if (predicate != null) { query = query.Where(predicate); }

            foreach(var include in includes.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(include);
            }

            if (orderBy != null) { query = orderBy(query); }

            if (skip.HasValue) { query = query.Skip(skip.Value); }
            if (take.HasValue) { query = query.Take(take.Value); }

            return query;
        }
    }
}
