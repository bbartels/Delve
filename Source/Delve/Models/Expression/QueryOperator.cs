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
        StartsWith,
        EndsWith
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
            { "^", QueryOperator.StartsWith },
            { "$", QueryOperator.EndsWith }
        };

        private static readonly Dictionary<QueryOperator, string> _formatMaps = new Dictionary<QueryOperator, string>
        {
            { QueryOperator.Equal,              "{0}=={1}"},
            { QueryOperator.NotEqual,           "{0}!={1}"},
            { QueryOperator.GreaterThan,        "{0}>{1}"},
            { QueryOperator.LessThan,           "{0}<{1}" },
            { QueryOperator.GreaterThanOrEqual, "{0}>={1}" },
            { QueryOperator.LessThanOrEqual,    "{0}<={1}" },
            { QueryOperator.Contains,           "{0}.Contains({1})" },
            { QueryOperator.StartsWith,         "{0}.StartsWith({1})" },
            { QueryOperator.EndsWith,           "{0}.EndsWith({1})" }
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

        public static string ConstructSubQuery(this QueryOperator op, string left, string right)
        {
            return string.Format(_formatMaps[op], left, right);
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
