using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Delve.Models.Expressions
{
    internal static class ExpressionFactory
    {
        private static readonly (Type type, Func<object, object, bool> func)[] funcs =
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
            (typeof(string), (x, y) => ((string)(object)y).EndsWith((string)(object)x, StringComparison.OrdinalIgnoreCase)),
            (typeof(IEnumerable), test)
        };

        private static bool test(object x, object y)
        {
            return ((IEnumerable)y).Cast<object>().Contains(x);
        }


        public static Func<object, object, bool> RequestFunc(QueryOperator op, Type type)
        {
            var func = funcs[(int)op];
            if (!func.type.IsAssignableFrom(type))
            {
                throw new InvalidQueryException($"Cannot use operator: '{op}' with type: '{type}'");
            }

            return funcs[(int)op].func;
        }
    }
}
