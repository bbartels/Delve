using System;
using System.Linq;

using Delve.Models.Validation;

namespace Delve.Models.Expressions
{
    internal class ExpandExpression<T, TResult> : BaseExpression<T, TResult>
    {
        public override string Query { get { return Key; } }

        public ExpandExpression(string key)
        {
            Key = QuerySanitizer.GetKey(ValidationType.Expand, key);
        }

        public override IQueryable<T> ApplyExpand(IQueryable<T> source, Func<IQueryable<T>, string, IQueryable<T>> include)
        {
            return include != null ? include(source, Key) : source;
        }
    }
}
