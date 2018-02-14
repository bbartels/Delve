using System;
using System.Linq;
using Delve.Models.Validation;

namespace Delve.Models.Expression
{
    internal class ExpandExpression<T, TResult> : BaseExpression<T, TResult>
    {
        public override string Query { get { return Key; } }

        public ExpandExpression(string key)
        {
            Key = QuerySanitizer.GetKey(ValidationType.Expand, key);
        }

        public override IQueryable<T> Apply(IQueryable<T> source, 
            Func<IQueryable<T>, string, IQueryable<T>> customFunc = null)
        {
            return customFunc != null ? customFunc(source, Key) : source;
        }
    }
}
