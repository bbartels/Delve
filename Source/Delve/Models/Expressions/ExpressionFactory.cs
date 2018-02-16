using System;

namespace Delve.Models.Expressions
{
    internal static class ExpressionFactory<TResult>
    {
        private static readonly (Type type, Func<TResult, TResult, bool> func)[] funcs =
        {
            (typeof(object), (x, y) => x.Equals(y)),
            (typeof(string), (x, y) => ((string)(object)x).Equals((string)(object)y, StringComparison.OrdinalIgnoreCase)),
            (typeof(object), (x, y) => !x.Equals(y)),
            (typeof(string), (x, y) => !((string)(object)x).Equals((string)(object)y, StringComparison.OrdinalIgnoreCase)),
            (typeof(IComparable), (x, y) => ((IComparable)y).CompareTo(x) > 0),
            (typeof(IComparable), (x, y) => ((IComparable)y).CompareTo(x) < 0),
            (typeof(IComparable), (x, y) => ((IComparable)y).CompareTo(x) >= 0),
            (typeof(IComparable), (x, y) => ((IComparable)y).CompareTo(x) <= 0),
            (typeof(string), (x, y) => ((string)(object)y).IndexOf((string)(object)x, StringComparison.Ordinal) != -1),
            (typeof(string), (x, y) => ((string)(object)y).IndexOf((string)(object)x, StringComparison.OrdinalIgnoreCase) != -1),
            (typeof(string), (x, y) => ((string)(object)y).StartsWith((string)(object)x, StringComparison.Ordinal)),
            (typeof(string), (x, y) => ((string)(object)y).StartsWith((string)(object)x, StringComparison.OrdinalIgnoreCase)),
            (typeof(string), (x, y) => ((string)(object)y).EndsWith((string)(object)x, StringComparison.Ordinal)),
            (typeof(string), (x, y) => ((string)(object)y).EndsWith((string)(object)x, StringComparison.OrdinalIgnoreCase))
            //(typeof(IEnumerable<TResult>), (x, y) => ((IEnumerable<TResult>)y).Contains(x))
        };

        public static Func<TResult, TResult, bool> RequestFunc(QueryOperator op, Type type)
        {
            var func = funcs[(int)op];
            if (!func.type.IsAssignableFrom(type))
            {
                throw new InvalidQueryException($"Cannot use operator: {op} with type: {type}");
            }

            return funcs[(int)op].func;
        }
    }
}
