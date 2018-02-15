using System;

namespace Delve.Models.Expressions
{
    internal static class ExpressionFactory<TResult>
    {
        private static readonly Func<TResult, TResult, bool>[] funcs =
        {
            (x, y) => x.Equals(y),
            (x, y) => ((string)(object)x).Equals((string)(object)y, StringComparison.OrdinalIgnoreCase),
            (x, y) => !x.Equals(y),
            (x, y) => !((string)(object)x).Equals((string)(object)y, StringComparison.OrdinalIgnoreCase),
            (x, y) => ((IComparable)y).CompareTo(x) > 0,
            (x, y) => ((IComparable)y).CompareTo(x) < 0,
            (x, y) => ((IComparable)y).CompareTo(x) >= 0,
            (x, y) => ((IComparable)y).CompareTo(x) <= 0,
            (x, y) => ((string)(object)y).IndexOf((string)(object)x, StringComparison.Ordinal) != -1,
            (x, y) => ((string)(object)y).IndexOf((string)(object)x, StringComparison.OrdinalIgnoreCase) != -1,
            (x, y) => ((string)(object)y).StartsWith((string)(object)x, StringComparison.Ordinal),
            (x, y) => ((string)(object)y).StartsWith((string)(object)x, StringComparison.OrdinalIgnoreCase),
            (x, y) => ((string)(object)y).EndsWith((string)(object)x, StringComparison.Ordinal),
            (x, y) => ((string)(object)y).EndsWith((string)(object)x, StringComparison.OrdinalIgnoreCase)
        };

        public static Func<TResult, TResult, bool> RequestFunc(QueryOperator op)
        {
            return funcs[(int)op];
        }
    }
}
