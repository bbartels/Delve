using System;
using System.Collections.Generic;
using System.Linq;

namespace Delve.Models
{
    internal interface IInternalResourceParameter<T>
    {
        IQueryable<T> ApplyFilters(IQueryable<T> source);
        IQueryable<T> ApplyOrderBy(IQueryable<T> source);
        IQueryable<T> ApplyExpand(IQueryable<T> source, Func<IQueryable<T>, string, IQueryable<T>> Include);
        dynamic ApplySelect(IEnumerable<T> source);
    }
}
