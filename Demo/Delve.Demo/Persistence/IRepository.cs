using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Delve.Models;

namespace Delve.Demo.Persistence
{
    public interface IRepository<T> where T : class
    {
        T GetById(int id);

        Task<T> GetByIdAsync(int id);

        IEnumerable<T> Get(Expression<Func<T, bool>> predicate = null, 
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, 
            string includeProperties = null,
            int? skip = null,
            int? take = null);

        Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> predicate = null, 
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, 
            string includes = null,
            int? skip = null,
            int? take = null);

        IPagedResult<T> Get(IResourceParameter<T> parameters);

        Task<IPagedResult<T>> GetAsync(IResourceParameter<T> parameters);

        T SingleOrDefault(Expression<Func<T, bool>> predicate);
        
        Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate);


        int GetCount(Expression<Func<T, bool>> predicate = null);

        Task<int> GetCountAsync(Expression<Func<T, bool>> predicate = null);
        
        bool GetExists(Expression<Func<T, bool>> predicate = null);

        Task<bool> GetExistsAsync(Expression<Func<T, bool>> predicate = null);

        void Add(T entity);

        void AddRange(IEnumerable<T> entities);
        
        void Remove(T entity);

        void RemoveRange(IEnumerable<T> entities);
    }
}
