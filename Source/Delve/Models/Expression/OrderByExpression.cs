using System.Collections.Generic;
using System.Linq;

using Delve.Models.Expression;
using Delve.Models.Validation;

namespace Delve.Models
{
    internal class OrderByExpression : INonValueExpression
    {
        public string PropertyExpression { get; private set; }
        public string Key { get; }
        public bool Descending { get; }

        public string Query
        {
            get { return $"{(Descending ? "-" : "+")}{Key}"; }
        }

        public OrderByExpression(string orderBy)
        {
            if (orderBy.StartsWith("-"))
            {
                Descending = true;
                orderBy = orderBy.Substring(1, orderBy.Length - 1);
            }
            else { Descending = false; }

            Key = orderBy;
        }

        public void ValidateExpression(IQueryValidator validator)
        {
            PropertyExpression = ((IInternalQueryValidator)validator).ValidateExpression(this, ValidationType.OrderBy);
        }

        public string GetDynamicLinqQuery()
        {
            return $"{PropertyExpression} {(Descending ? "DESC" : "ASC")}";
        }

        public static string GetDynamicLinqQuery(IList<INonValueExpression> orderBy)
        {
            if (orderBy == null || orderBy.Count == 0) { return null; }

            return orderBy.Select(ob => ob.GetDynamicLinqQuery()).Aggregate((x, y) => $"{x},{y}");
        }
    }
}
