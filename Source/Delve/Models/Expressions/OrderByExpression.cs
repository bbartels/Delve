using System.Linq;

using Delve.Models.Validation;

namespace Delve.Models.Expressions
{
    internal class OrderByExpression<T, TResult> : BaseExpression<T, TResult>
    {
        public bool Descending { get; }

        public override string Query { get { return $"{(Descending ? "-" : "")}{Key}"; } }

        public OrderByExpression(string orderBy)
        {
            if (orderBy.StartsWith("-"))
            {
                Descending = true;
            }

            Key = QuerySanitizer.GetKey(ValidationType.OrderBy, orderBy);
        }

        public override IQueryable<T> ApplySort(IQueryable<T> source, bool thenBy)
        {
            var property = Property.Compile();

            if (!thenBy)
            {
                return Descending
                    ? source.OrderByDescending(x => property(x)) 
                    : source.OrderBy(x => property(x));
            }

            var orderedSource = source as IOrderedQueryable<T>;

            return Descending
                ? orderedSource.ThenByDescending(x => property(x))
                : orderedSource.ThenBy(x => property(x));
        }
    }
}
