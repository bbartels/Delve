using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Delve.Models.Expression;

namespace Delve.Models
{
    internal class SelectExpression<T, TResult> : BaseExpression<T, TResult>, ISelectExpression<T>
    {
        public override string Query { get { return Key; } }

        public SelectExpression(string select)
        {
            Key = select;
        }

        public override IQueryable<T> Apply(IQueryable<T> source, Func<IQueryable<T>, string, IQueryable<T>> customFunc = null)
        {
            return source.Select(x => x);
        }

        public dynamic ApplySelect(IEnumerable<T> source)
        {
            return source.Select(x => Property(x));
        }
    }
}
