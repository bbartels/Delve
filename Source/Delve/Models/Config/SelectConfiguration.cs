using System;
using System.Linq.Expressions;

namespace Delve.Models
{
    internal class SelectConfiguration<T, TProperty> : ISelectConfiguration<T>
    {
        private readonly string _key;
        private readonly Expression<Func<T, TProperty>> _expression;

        public SelectConfiguration(string key, Expression<Func<T, TProperty>> expression)
        {
            _key = key;
            _expression = expression;
        }


        public Func<T, (string, object)> GetPropertyMapping()
        {
            try
            {
                return t => (_key, _expression.Compile()(t));
            }

            catch (ArgumentNullException)
            {
                throw new InvalidQueryException($"Could not select requested property: {_key}." +
                                                "Likely cause by missing Expand.");
            }
        }
    }
}
