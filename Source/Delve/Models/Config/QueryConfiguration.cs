using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;

using Delve.Models.Validation;

namespace Delve.Models
{
    internal class QueryConfiguration<T> : IQueryConfiguration<T>
    {
        private readonly IList<Expression<Func<T, bool>>> _defaultFilters 
                                            = new List<Expression<Func<T, bool>>>();
        private readonly IList<ISortConfiguration<T>> _defaultSorts
                                            = new List<ISortConfiguration<T>>();
        private readonly IList<ISelectConfiguration<T>> _defaultSelects
                                            = new List<ISelectConfiguration<T>>();

        private readonly IList<string> _defaultExpands = new List<string>();

        public void AddDefaultFilter(Expression<Func<T, bool>> exp)
        {
            _defaultFilters.Add(exp);
        }

        public void AddDefaultSelect<TProperty>(string key, Expression<Func<T, TProperty>> exp)
        {
            _defaultSelects.Add(new SelectConfiguration<T, TProperty>(key, exp));
        }

        public void AddDefaultSort<TProperty>(Expression<Func<T, TProperty>> exp, bool descending)
        {
            _defaultSorts.Add(new SortConfiguration<T, TProperty>(exp, descending));
        }

        public void AddDefaultExpand<TProperty>(Expression<Func<T, TProperty>> exp)
        {
            _defaultExpands.Add(ValidatorHelpers.GetExpandString(exp.ToString()));
        }

        public IQueryable<T> ApplyDefaultFilters(IQueryable<T> source)
        {
            return source == null 
                ? null 
                : _defaultFilters.Aggregate(source, (current, filter) => current.Where(filter));
        }

        public IQueryable<T> ApplyDefaultExpands(IQueryable<T> source, Func<IQueryable<T>, string, IQueryable<T>> include)
        {
            return source == null ? null : _defaultExpands.Aggregate(source, include);
        }

        public IList<object> ApplyDefaultSelects(IEnumerable<T> source)
        {
            if (_defaultSelects.Count == 0)
            {
                return source.Select(x => (object)x).ToList();
            }

            var test = _defaultSelects.Select(x => x.GetPropertyMapping());

            object applyFunc(T user, IEnumerable<Func<T, (string, object)>> func)
            {
                var t = new ExpandoObject() as IDictionary<string, object>;
                foreach (var f in func)
                {
                    var elements = f(user);
                    t.Add(elements.Item1, elements.Item2);
                }

                return (ExpandoObject)t;
            }

            return source.Select(x => applyFunc(x, test)).ToList();
        }

        public IQueryable<T> ApplyDefaultSorts(IQueryable<T> source)
        {
            if (source == null || _defaultSorts.Count == 0) { return source; }

            var orderedSort = _defaultSorts[0].ApplySort(source);

            for (int i = 1; i < _defaultSorts.Count; i++)
            {
                orderedSort = _defaultSorts[i].ApplySort(orderedSort);
            }

            return orderedSort;
        }
    }
}
