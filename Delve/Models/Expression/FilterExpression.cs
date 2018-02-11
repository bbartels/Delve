using System.Linq;
using System.Collections.Generic;

namespace Delve.Models
{
    public class FilterExpression
    {
        public string LeftOperand { get; }
        public QueryOperator Operator { get; }
        public string[] RightOperand { get; }

        public string Query
        {
            get { return $"{LeftOperand}{Operator.GetSymbol()}{RightOperand.Aggregate((x, y) => $"{x},{y}")}"; }
        }

        public FilterExpression(string filter)
        {
            Operator = filter.GetQueryOperator();
            var (op1, op2) = filter.GetOperands();
            LeftOperand = op1;
            RightOperand = SplitOperand(op2);
        }

        public string[] SplitOperand(string op)
        {
            return op.Split('|').Select(property => property.Trim()).ToArray();
        }

        public (string, string[]) GetDynamicLinqQuery(int startIdx = 0)
        {
            string query = string.Join(" or ", RightOperand
                .Select((o, i) => Operator.ConstructSubQuery(LeftOperand, $"@{i + startIdx}")));

            query = $"({query})";

            return (query, RightOperand);
        }

        public static IEnumerable<FilterExpression> ParseExpression(string filters)
        {
            return filters == null ? Enumerable.Empty<FilterExpression>() : 
                    filters.Split(',').Select(f => new FilterExpression(f.Trim()));
        }

        public static string GetQuery(IEnumerable<FilterExpression> filters)
        {
            return filters.Select(x => x.Query).Aggregate((x, y) => $"{x},{y}");
        }

        public static (string, object[]) GetDynamicLinqQuery(IList<FilterExpression> filters)
        {
            if (filters.Count == 0) { return (null, null); }
            var values = filters.Select(f => f.GetDynamicLinqQuery().Item2)
                .SelectMany(x => x).Select(x => (object)x).ToArray();

            var subQueries = new List<string>();
            int count = 0;
            foreach (var filter in filters)
            {
                var linqQuery = filter.GetDynamicLinqQuery(count);
                subQueries.Add(linqQuery.Item1);
                count += linqQuery.Item2.Length;
            }
            string query = string.Join(" and ", subQueries);

            return (query, values);
        }
    }
}
