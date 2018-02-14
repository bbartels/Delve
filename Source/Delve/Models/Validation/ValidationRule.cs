using System;
using System.Linq.Expressions;

using Delve.Models.Expression;

namespace Delve.Models.Validation
{
    internal class ValidationRule<T, TResult> : IValidationRule
    {
        public ValidationType ValidationType { get; }

        public Type ResultType
        {
            get { return typeof(TResult); }
        }

        public Expression<Func<T, TResult>> Expression { get; private set; }

        public ValidationRule(Expression<Func<T, TResult>> exp, ValidationType type)
        {
            Expression = exp;
            if (type == ValidationType.Expand)
            {
                ValidatorHelpers.CheckExpressionIsProperty(exp);
            }
            ValidationType = type;
        }

        public IValidationRule ValidateExpression(IExpression exp)
        {
            if (exp.PropertyType != Expression.ReturnType)
            {
                throw new InvalidQueryException($"Values of {exp.Key} do not match Type: {Expression.ReturnType}.");
            }
            return this;
        }
    }
}
