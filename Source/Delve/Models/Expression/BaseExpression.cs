using System;
using System.Linq;

using Delve.Models.Validation;

namespace Delve.Models.Expression
{
    internal abstract class BaseExpression<T, TResult> : IExpression<T>
    {
        public string Key { get; protected set; }
        public abstract string Query { get; }
        public Type PropertyType { get { return typeof(TResult); } }
        public Func<T, TResult> Property;

        public abstract IQueryable<T> Apply(IQueryable<T> source,
            Func<IQueryable<T>, string, IQueryable<T>> customFunc = null);

        public void ValidateExpression(IQueryValidator validator)
        {
            var rule = ((IInternalQueryValidator<T>)validator).ValidateExpression(this, IExpressionExtensions.GetValidationType(GetType()));

            try
            {
                Property = ((ValidationRule<T, TResult>)rule).Expression.Compile();
            }

            catch (InvalidCastException)
            {
                throw new RuntimeException("Could not cast validationrule.");
            }
        }
    }
}
