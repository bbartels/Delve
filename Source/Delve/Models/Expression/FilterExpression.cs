using System.Linq;
using System.Collections.Generic;

using Delve.Models.Expression;
using Delve.Models.Validation;

namespace Delve.Models
{
    internal class FilterExpression : IValueExpression
    {
        public string PropertyExpression { get; protected set; }
        public string Key { get; }
        public QueryOperator Operator { get; }
        public string[] Values { get; }

        public string Query
        {
            get { return $"{Key}{Operator.GetSymbol()}{Values.Aggregate((x, y) => $"{x},{y}")}"; }
        }

        public FilterExpression(string filter)
        {
            Operator = filter.GetQueryOperator();
            var (op1, op2) = filter.GetOperands();
            Values = op2.Split('|').Select(property => property.Trim()).ToArray();
            Key = op1;
        }

        public void ValidateExpression(IQueryValidator validator)
        {
            PropertyExpression = ((IInternalQueryValidator)validator).ValidateValueExpression(this, ValidationType.Filter);
        }

        public (string query, string[] values) GetDynamicLinqQuery(int startIndex = 0)
        {
            if (PropertyExpression == null)
            {
                throw new RuntimeException("PropertyExpression has not been validated.");
            }

            string query = string.Join(" or ", Values
                .Select((o, i) => Operator.ConstructSubQuery(PropertyExpression, $"@{i + startIndex}")));

            query = $"({query})";

            return (query, Values);
        }

        public static (string, object[]) GetDynamicLinqQuery(IList<IValueExpression> filters)
        {
            if (filters.Count == 0) { return (null, null); }
            var values = filters.Select(f => f.GetDynamicLinqQuery().query)
                .SelectMany(x => x).Select(x => (object)x).ToArray();

            var subQueries = new List<string>();
            int count = 0;
            foreach (var filter in filters)
            {
                var linqQuery = filter.GetDynamicLinqQuery(count);
                subQueries.Add(linqQuery.query);
                count += linqQuery.values.Length;
            }
            string query = string.Join(" and ", subQueries);

            return (query, values);
        }
    }
}
