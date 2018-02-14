using System;
using System.Linq;

namespace Delve.Models.Expression
{
    internal class ExpandExpression<T, TResult> : BaseExpression<T, TResult>
    {
        public override string Query { get { return Key; } }

        public ExpandExpression(string key)
        {
            Key = key;
        }

        public override IQueryable<T> Apply(IQueryable<T> source, 
            Func<IQueryable<T>, string, IQueryable<T>> customFunc = null)
        {
            return customFunc != null ? customFunc(source, Key) : source;
        }
    }
}
