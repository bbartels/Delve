using System;

namespace Delve.Models.Expressions
{
    internal class SelectExpression<T, TResult> : BaseExpression<T, TResult>
    {
        public override string Query { get { return Key; } }

        public SelectExpression(string select)
        {
            Key = select;
        }

        public override Func<T, (string, object)> GetPropertyMapping()
        {
            try
            {
                return (t) => (Key, Property.Compile()(t));
            }

            catch (ArgumentNullException)
            {
                throw new InvalidQueryException($"Could not select requested property: {Key}." +
                                                "Likely cause by missing Expand.");
            }
        }
    }
}
