using System;
using System.Collections.Generic;
using System.Linq;

using Delve.Models.Expression;
using Delve.Models.Validation;

namespace Delve.Models
{
    internal class SelectExpression : INonValueExpression
    {
        public string PropertyExpression { get; private set; }
        public string Key { get; }
        public string Query { get { return Key; } }

        public SelectExpression(string select)
        {
            Key = select;
        }

        public void ValidateExpression(IQueryValidator validator)
        {
            PropertyExpression = ((IInternalQueryValidator)validator).ValidateExpression(this, ValidationType.Select);
        }

        public string GetDynamicLinqQuery()
        {
            var expression = PropertyExpression.Split(new[] { "=>" }, StringSplitOptions.None);
            var lambda = expression[0].Trim();

            return $"{expression[1].Replace($"{lambda}.", "")} as {Key}";
        }

        public static string GetDynamicLinqQuery(IList<INonValueExpression> select)
        {
            if (select == null || select.Count == 0) { return null; }

            return $"new ({select.Select(ob => ob.GetDynamicLinqQuery()).Aggregate((x, y) => $"{x},{y}")})";
        }
    }
}
