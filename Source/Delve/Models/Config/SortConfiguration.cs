using System;
using System.Linq;
using System.Linq.Expressions;

namespace Delve.Models
{
    internal class SortConfiguration<T, TProperty> : ISortConfiguration<T>
    {
        private readonly Expression<Func<T, TProperty>> _expression;
        private readonly bool _descending;

        public SortConfiguration(Expression<Func<T, TProperty>> expression, bool descending)
        {
            _expression = expression;
            _descending = descending;
        }

        public IOrderedQueryable<T> ApplySort(IQueryable<T> source)
        {
            if (source == null) { return null; }
            return _descending 
                ? source.OrderByDescending(_expression)
                : source.OrderBy(_expression);
        }

        public IOrderedQueryable<T> ApplySort(IOrderedQueryable<T> source)
        {
            if (source == null) { return null; }
            return _descending 
                ? source.ThenByDescending(_expression)
                : source.ThenBy(_expression);
        }
    }
}
