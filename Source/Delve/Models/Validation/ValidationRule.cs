using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;

using Delve.Models.Expression;

namespace Delve.Models.Validation
{
    internal class ValidationRule<T, TResult> : IValidationRule
    {
        public ValidationType ValidationType { get; }
       

        public Expression<Func<T, TResult>> Expression { get; private set; }

        public ValidationRule(Expression<Func<T, TResult>> exp, ValidationType type)
        {
            Expression = exp;
            ValidationType = ValidationType;
        }

        public string ValidateExpression(INonValueExpression expression)
        {
            return Expression.ToString();
        }

        public string ValidateValueExpression(IValueExpression expression)
        {
            ValidatePropertyType(expression.Key, expression.Values);
            return Expression.ToString();
        }

        private void ValidatePropertyType(string key, IEnumerable<string> values)
        {
            if (Expression.ReturnType == typeof(string)) { return; }

            var currentValue = string.Empty;
            try
            {
                var converter = TypeDescriptor.GetConverter(Expression.ReturnType);
                foreach (var value in values)
                {
                    currentValue = value;
                    converter?.ConvertFromString(value);
                }
            }
            catch (NotSupportedException)
            {
                throw new InvalidQueryException($"Value: {currentValue}: does not match " +
                                    $"datatype: {Expression.ReturnType} of registered key: {key}");
            }
        }
    }
}
