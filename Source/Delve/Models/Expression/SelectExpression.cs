using System;
using Delve.Models.Expression;
using Delve.Models.Validation;

namespace Delve.Models
{
    public class SelectExpression : INonValueExpression
    {
        public string PropertyExpression { get; protected set; }
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
            throw new NotImplementedException();
        }
    }
}
