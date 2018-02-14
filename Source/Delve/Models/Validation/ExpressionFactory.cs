using System;
using System.Linq.Expressions;

namespace Delve.Models
{
    internal static class ExpressionFactory<TResult>
    {
        private static readonly Expression<Func<TResult, TResult, bool>>[] funcs =
        {
            (x, y) => x.Equals(y),
            (x, y) => !x.Equals(y),
            (x, y) => ((IComparable)x).CompareTo(y) > 0,
            (x, y) => ((IComparable)x).CompareTo(y) < 0,
            (x, y) => ((IComparable)x).CompareTo(y) >= 0,
            (x, y) => ((IComparable)x).CompareTo(y) <= 0,
            (x, y) => ((string)(object)x).IndexOf((string)(object)y, StringComparison.Ordinal) != -1,
            (x, y) => ((string)(object)x).StartsWith((string)(object)y, StringComparison.Ordinal),
            (x, y) => ((string)(object)x).EndsWith((string)(object)y, StringComparison.Ordinal),
            (x, y) => ((string)(object)x).Equals((string)(object)y, StringComparison.OrdinalIgnoreCase),
            (x, y) => ((string)(object)x).IndexOf((string)(object)y, StringComparison.OrdinalIgnoreCase) != -1,
            (x, y) => ((string)(object)x).StartsWith((string)(object)y, StringComparison.OrdinalIgnoreCase),
            (x, y) => ((string)(object)x).EndsWith((string)(object)y, StringComparison.OrdinalIgnoreCase)
        };

        public static Expression<Func<TResult, TResult, bool>> RequestFunc(QueryOperator op)
        {
            return funcs[(int)op];
        }
    }
}
