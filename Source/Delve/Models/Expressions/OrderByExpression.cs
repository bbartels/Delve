using System;
using System.Linq;

using Delve.Models.Expressions;
using Delve.Models.Validation;

namespace Delve.Models
{
    internal class OrderByExpression<T, TResult> : BaseExpression<T, TResult>
    {
        public bool Descending { get; }

        public override string Query { get { return $"{(Descending ? "-" : "+")}{Key}"; } }

        public OrderByExpression(string orderBy)
        {
            if (orderBy.StartsWith("-"))
            {
                Descending = true;
            }

            Key = QuerySanitizer.GetKey(ValidationType.OrderBy, orderBy);
        }

        public override IQueryable<T> Apply(IQueryable<T> source, 
            Func<IQueryable<T>, string, IQueryable<T>> customFunc = null)
        {
            return Descending ? source.OrderByDescending(x => Property.Compile()(x)) : source.OrderBy(x => Property.Compile()(x));
        }
    }
}
