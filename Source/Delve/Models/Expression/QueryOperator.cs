using System;
using System.Collections.Generic;
using System.Linq;

namespace Delve.Models
{
    internal enum QueryOperator
    {
        Equal,
        NotEqual,
        GreaterThan,
        LessThan,
        GreaterThanOrEqual,
        LessThanOrEqual,
        Contains,
        ContainsInsensitive,
        StartsWith,
        StartsWithInsensitive,
        EndsWith,
        EndsWithInsensitive
    }

    internal static class QueryOperatorExtensions
    {
        private static readonly Dictionary<string, QueryOperator> _operatorMaps = new Dictionary<string, QueryOperator>
        {
            { "==", QueryOperator.Equal },
            { "!=", QueryOperator.NotEqual },
            { ">", QueryOperator.GreaterThan },
            { "<", QueryOperator.LessThan},
            { ">=", QueryOperator.GreaterThanOrEqual },
            { "<=", QueryOperator.LessThanOrEqual },
            { "?", QueryOperator.Contains },
            { "*?", QueryOperator.ContainsInsensitive },
            { "*^", QueryOperator.StartsWithInsensitive },
            { "*$", QueryOperator.EndsWithInsensitive }
        };

        public static string GetSymbol(this QueryOperator value)
        {
            return _operatorMaps.FirstOrDefault(k => k.Value == value).Key;
        }

        public static (string o1, string o2) GetOperands(this string str)
        {
            var operands = (string.Empty, string.Empty);
            foreach (var op in _operatorMaps)
            {
                int index = str.IndexOf(op.Key, StringComparison.Ordinal);
                if (index == -1) { continue; }

                operands.Item1 = str.Substring(0, index);
                operands.Item2 = str.Substring(index + op.Key.Length, str.Length - index - op.Key.Length);
            }

            if (operands.Item1 == string.Empty || operands.Item2 == string.Empty)
            {
                throw new InvalidQueryException($"{str} does not contain valid operands.");
            }

            return operands;
        }

        public static QueryOperator GetQueryOperator(this string str)
        {
            foreach (var op in _operatorMaps)
            {
                if (str.Contains(op.Key)) { return op.Value; }
            }
            throw new InvalidQueryException($"{str} does not contain a valid {nameof(QueryOperator)}.");
        }
    }
}
