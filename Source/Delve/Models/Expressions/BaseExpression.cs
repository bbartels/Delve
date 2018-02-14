using System;
using System.Linq;
using System.Linq.Expressions;

using Delve.Models.Validation;

namespace Delve.Models.Expressions
{
    internal abstract class BaseExpression<T, TResult> : IExpression<T>
    {
        public string Key { get; protected set; }
        public abstract string Query { get; }
        public Type PropertyType { get { return typeof(TResult); } }
        public Expression<Func<T, TResult>> Property;

        public void ValidateExpression(IQueryValidator validator)
        {
            var rule = ((IInternalQueryValidator<T>)validator).ValidateExpression(this, IExpressionExtensions.GetValidationType(GetType()));

            try
            {
                Property = ((ValidationRule<T, TResult>)rule).Expression;
            }

            catch (InvalidCastException)
            {
                throw new RuntimeException("Could not cast validationrule.");
            }
        }

        public virtual IQueryable<T> ApplyFilter(IQueryable<T> source)
        {
            throw new RuntimeException("This method should not be called. Please report to github.com/bbartels/delve.");
        }

        public virtual IQueryable<T> ApplySort(IQueryable<T> source, bool thenBy)
        {
            throw new RuntimeException("This method should not be called. Please report to github.com/bbartels/delve.");
        }

        public virtual IQueryable<T> ApplyExpand(IQueryable<T> source, Func<IQueryable<T>, string, IQueryable<T>> include)
        {
            throw new RuntimeException("This method should not be called. Please report to github.com/bbartels/delve.");
        }

        public virtual string GetSelectProperty()
        {
            throw new RuntimeException("This method should not be called. Please report to github.com/bbartels/delve.");
        }

        public virtual Func<T, (string, object)> GetPropertyMapping()
        {
            throw new RuntimeException("This method should not be called. Please report to github.com/bbartels/delve.");
        }

        protected string GetPropertyName()
        {
            var propString = Property.ToString();
            return propString.Substring(propString.IndexOf('.') + 1).TrimEnd(')');
        }
    }
}
