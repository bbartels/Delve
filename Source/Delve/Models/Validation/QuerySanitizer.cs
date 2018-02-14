using System;
using System.Collections.Generic;
using System.Linq;

namespace Delve.Models.Validation
{
    internal static class QuerySanitizer
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
            { "^", QueryOperator.StartsWith },
            { "$", QueryOperator.EndsWith },
            { "*?", QueryOperator.ContainsInsensitive },
            { "*^", QueryOperator.StartsWithInsensitive },
            { "*$", QueryOperator.EndsWithInsensitive }
        };

        public static string GetKey(ValidationType type, string query)
        {
            query = query.Trim();

            switch (type)
            {
                case ValidationType.Select:
                {
                    return query;
                }
                case ValidationType.OrderBy:
                {
                    return query[0] == '-' ? query.Substring(1, query.Length - 1) : query;
                }
                case ValidationType.Filter:
                {
                    return GetOperands(query).o1;
                }
                case ValidationType.Expand:
                {
                    return query;
                }
                default:
                {
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
                }
            }
        }

        public static string[] GetFilterValues(string query)
        {
            return GetOperands(query).o2.Split('|').Select(property => property.Trim()).ToArray();
        }

        public static string GetFilterSymbol(QueryOperator op)
        {
            return _operatorMaps.FirstOrDefault(k => k.Value == op).Key;
        }

        public static QueryOperator GetFilterOperator(string query)
        {
            foreach (var op in _operatorMaps)
            {
                if (query.Contains(op.Key)) { return op.Value; }
            }
            throw new InvalidQueryException($"{query} does not contain a valid {nameof(QueryOperator)}.");
        }

        private static (string o1, string o2) GetOperands(string str)
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
    }
}
