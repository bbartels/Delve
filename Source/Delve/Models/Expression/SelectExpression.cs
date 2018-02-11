using System.Collections.Generic;
using System.Linq;

namespace Delve.Models
{
    public class SelectExpression
    {
        public string Property { get; }

        public SelectExpression(string select)
        {
            Property = select;
        }
        
        public static IEnumerable<SelectExpression> ParseExpression(string selects)
        {
            return selects == null ? Enumerable.Empty<SelectExpression>() : 
                selects.Split(',').Select(s => new SelectExpression(s.Trim()));
        }

        public static string GetSelectQuery(IEnumerable<SelectExpression> selects)
        {
            return selects.Select(s => s.Property).Aggregate((x, y) => $"{x},{y}");
        }
    }
}
