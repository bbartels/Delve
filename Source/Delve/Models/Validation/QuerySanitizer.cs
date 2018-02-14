using System;
using System.Collections.Generic;
using System.Text;

namespace Delve.Models.Validation
{
    internal static class QuerySanitizer
    {
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
                    return "".GetOperands().o1;
                }
                case ValidationType.Expand:
                {
                    return query;
                }
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}
