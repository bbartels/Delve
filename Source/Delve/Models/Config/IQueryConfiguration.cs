﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Delve.Models
{
    internal interface IQueryConfiguration<T>
    {
        IQueryable<T> ApplyDefaultFilters(IQueryable<T> source);
        IQueryable<T> ApplyDefaultSorts(IQueryable<T> source);
        IQueryable<T> ApplyDefaultExpands(IQueryable<T> source, Func<IQueryable<T>, string, IQueryable<T>> include);
        IList<object> ApplyDefaultSelects(IEnumerable<T> source);

        void AddDefaultFilter(Expression<Func<T, bool>> exp);
        void AddDefaultSort<TProperty>(Expression<Func<T, TProperty>> exp, bool descending);
        void AddDefaultSelect<TProperty>(string key, Expression<Func<T, TProperty>> exp);
        void AddDefaultExpand<TProperty>(Expression<Func<T, TProperty>> exp);
    }
}
