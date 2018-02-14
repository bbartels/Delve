using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

using Delve.Models.Expression;

namespace Delve.Models.Validation
{
    public class AbstractQueryValidator<T> : IQueryValidator<T>, IInternalQueryValidator<T>
    {
        private readonly IDictionary<string, ValidationRules> _validationRules = 
                new Dictionary<string, ValidationRules>(StringComparer.CurrentCultureIgnoreCase);

        private void AddRule<TResult>(string key, IValidationRule rule)
        {
            if (rule.ValidationType != ValidationType.Expand && !ValidatorHelpers.IsValidType(typeof(TResult)))
            {
                throw new InvalidValidationBuilderException($"Registered property ({key})" +
                                                            $" cannot be of type {typeof(TResult)}.");
            }

            if (rule.ValidationType == ValidationType.Expand && !ValidatorHelpers.IsValidExpandType(typeof(TResult)))
            {
                throw new InvalidValidationBuilderException($"Registered property ({key})" +
                                                            $" cannot be of type {typeof(TResult)}." +
                                                            $"Expand is limited to Enumerable Types.");
            }

            if (!Regex.IsMatch(key, @"^[a-zA-Z0-9_]+$"))
            {
                throw new InvalidValidationBuilderException("Key must only contain numbers, letters and underscores.");
            }

            if (!_validationRules.ContainsKey(key))
            {
                _validationRules.Add(key, new ValidationRules(rule)); 
            }

            else
            {
                _validationRules[key].AddRule(rule);
            }
        }

        protected void CanFilter<TResult>(string key, Expression<Func<T, TResult>> exp)
        {
            AddRule<TResult>(key, new ValidationRule<T, TResult>(exp, ValidationType.Filter));
        }

        protected void CanSelect<TResult>(string key, Expression<Func<T, TResult>> exp)
        {
            AddRule<TResult>(key, new ValidationRule<T, TResult>(exp, ValidationType.Select));
        }

        protected void CanOrder<TResult>(string key, Expression<Func<T, TResult>> exp)
        {
            AddRule<TResult>(key, new ValidationRule<T, TResult>(exp, ValidationType.OrderBy));
        }

        protected void CanExpand<TResult>(string key, Expression<Func<T, TResult>> exp) where TResult : class, IEnumerable
        {
            AddRule<TResult>(key, new ValidationRule<T, TResult>(exp, ValidationType.Expand));
        }

        private void ValidateKey(string key, ValidationType type)
        {
            if (!_validationRules.ContainsKey(key))
            {
                throw new InvalidQueryException($"Invalid {type} " +
                                            $"propertykey: {key} in query.");
            }
        }

        IValidationRule IInternalQueryValidator<T>.ValidateExpression(IExpression expression, ValidationType type)
        {
            ValidateKey(expression.Key, type);
            return _validationRules[expression.Key].ValidateExpression(type, expression);
        }

        Type IInternalQueryValidator<T>.GetResultType(string key, ValidationType type)
        {
            ValidateKey(key, type);
            return _validationRules[key].GetResultType(type, key);
        }
    }

    internal static class ValidatorHelpers
    {
        private static readonly Type[] _validNonPrimitive =
        {
            typeof(string), typeof(DateTime), typeof(TimeSpan)
        };

        public static bool IsValidType(Type type)
        {
            return type.IsPrimitive || _validNonPrimitive.Contains(type);
        }

        public static bool IsValidExpandType(Type type)
        {
            return typeof(IEnumerable).IsAssignableFrom(type);
        }

        public static string CheckExpressionIsProperty<T, TResult>(Expression<Func<T, TResult>> expression)
        {
            if (expression.Body is MemberExpression member)
            {
                return member.Member.Name;
            }
            throw new ArgumentException($"Lambda expression: {expression.Body} does not refer to a property.");
        }
    }
}
