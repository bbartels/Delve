using System.Collections.Generic;
using System.Linq;

namespace Delve.Models
{
    public class OrderByExpression
    {
        public string Property { get; }
        public bool Descending { get; }

        public string Query
        {
            get { return $"{(Descending ? "-" : "+")}{Property}"; }
        }

        public OrderByExpression(string orderBy)
        {
            if (orderBy.StartsWith("-"))
            {
                Descending = true;
                orderBy = orderBy.Substring(1, orderBy.Length - 1);
            }
            else { Descending = false; }

            Property = orderBy;
        }

        public string GetDynamicLinqQuery()
        {
            return $"{Property} {(Descending ? "DESC" : "ASC")}";
        }

        public static IEnumerable<OrderByExpression> ParseExpression(string orderBy)
        {
            return orderBy == null ? Enumerable.Empty<OrderByExpression>() : 
                orderBy.Split(',').Select(o => new OrderByExpression(o.Trim()));
        }

        public static string GetQuery(IEnumerable<OrderByExpression> orderBy)
        {
            return orderBy.Select(x => x.Property).Aggregate((x, y) => $"{x},{y}");
        }

        public static string GetDynamicLinqQuery(IList<OrderByExpression> orderBy)
        {
            if (orderBy == null || orderBy.Count == 0) { return null; }

            return orderBy.Select(ob => ob.GetDynamicLinqQuery()).Aggregate((x, y) => $"{x},{y}");
        }
    }
}
