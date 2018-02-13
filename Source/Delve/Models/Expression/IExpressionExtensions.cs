using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Delve.Models.Expression
{
    internal static class IExpressionExtensions
    {
        public static string GetQuery(this IEnumerable<IExpression> items)
        {
            return items.Select(x => x.Query).Aggregate((x, y) => $"{x},{y}");
        }

        public static IList<T> ParseQuery<T>(this string query, Func<string, T> create) where T : IExpression
        {
            return query == null ? new List<T>() : 
                query.Split(',').Select(s => create(s.Trim())).ToList();
        }
    }
}
