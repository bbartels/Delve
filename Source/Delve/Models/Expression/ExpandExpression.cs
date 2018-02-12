using Delve.Models.Validation;

namespace Delve.Models.Expression
{
    internal class ExpandExpression : INonValueExpression
    {
        public string PropertyExpression { get; private set; }
        public string Key { get; }
        public string Query
        {
            get { return Key; }
        }

        public ExpandExpression(string key)
        {
            Key = key;
        }

        public void ValidateExpression(IQueryValidator validator)
        {
            PropertyExpression = ((IInternalQueryValidator)validator).ValidateExpression(this, ValidationType.Expand);
        }

        public string GetDynamicLinqQuery()
        {
            return PropertyExpression;
        }
    }
}
