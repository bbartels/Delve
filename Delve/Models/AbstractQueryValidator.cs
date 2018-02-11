using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace Delve.Models
{
    public class AbstractQueryValidator<T> : IQueryValidator<T>
    {
        private readonly IDictionary<string, ValidationRule> _validationRules = new Dictionary<string, ValidationRule>(StringComparer.CurrentCultureIgnoreCase);

        private void AddRule<TProperty>(ValidationType type, string key, Expression<Func<T, TProperty>> exp)
        {
            key = key ?? GetPropertyName(exp);

            if (!ValidatorHelpers.IsValidType(typeof(TProperty)))
            {
                throw new ArgumentException($"Registered property ({key}) cannot be of type {typeof(TProperty)}.");
            }

            if (!Regex.IsMatch(key, @"^[a-zA-Z0-9_]+$")) { throw new ArgumentException("Property name must match regex ^[a-zA-Z0-9_]+$"); }

            if (!_validationRules.ContainsKey(key))
            {
                _validationRules.Add(key, new ValidationRule(type, GetPropertyName(exp), typeof(TProperty))); 
            }
            else
            {
                var rule = _validationRules[key];
                if (rule.PropertyName != GetPropertyName(exp) || rule.PropertyType != typeof(TProperty))
                {
                    throw new ArgumentException($"Cannot register mulitple rules with same key for different Properties.");
                }

                rule.Add(type);
            }
        }

        protected void CanOrder<TProperty>(Expression<Func<T, TProperty>> expression, string propertyKey = null)
        {
            AddRule(ValidationType.OrderBy, propertyKey, expression);
        }

        protected void AddToSearch<TProperty>(Expression<Func<T, TProperty>> expression, string propertyKey = null)
        {
            AddRule(ValidationType.Search, propertyKey, expression);
        }

        protected void CanFilter<TProperty>(Expression<Func<T, TProperty>> expression, string propertyKey = null)
        {
            AddRule(ValidationType.Filter, propertyKey, expression);
        }

        protected void CanSelect<TProperty>(Expression<Func<T, TProperty>> expression, string propertyKey = null)
        {
            AddRule(ValidationType.Select, propertyKey, expression);
        }

        public void Validate(IResourceParameter parameters)
        {
            foreach (var orderBy in parameters.OrderBy)
            {
                ValidateValidationType(orderBy.Property, ValidationType.OrderBy);
            }
            foreach (var filter in parameters.Filter)
            {
                ValidateValidationType(filter.LeftOperand, ValidationType.Filter);
                ValidatePropertyType(filter.LeftOperand, filter.RightOperand);
            }
            foreach (var select in parameters.Select)
            {
                ValidateValidationType(select.Property, ValidationType.Select);
            }
        }

        private void ValidateValidationType(string key, ValidationType type)
        {
            if (!_validationRules.ContainsKey(key))
            {
                throw new InvalidQueryException($"Invalid {ValidatorHelpers.TypeDesc[type]} " +
                                                $"propertykey: {key} in query.");
            }

            if (!_validationRules[key].Contains(type))
            {
                throw new InvalidQueryException($"{ValidatorHelpers.TypeDesc[type]} mapping on " +
                                                $"key: {key} does not exist on type: {typeof(T)}.");
            }
        }

        private void ValidatePropertyType(string key, string[] values)
        {
            var type = _validationRules[key].PropertyType;
            if (type == typeof(string)) { return; }

            var currentValue = string.Empty;
            try
            {
                var converter = TypeDescriptor.GetConverter(type);
                foreach (var value in values)
                {
                    currentValue = value;
                    converter?.ConvertFromString(value);
                }
            }
            catch (NotSupportedException)
            {
                throw new InvalidQueryException($"Value: {currentValue}: does not match " +
                                                $"datatype: {type} of registered key: {key}");
            }
        }

        private string GetPropertyName<TProperty>(Expression<Func<T, TProperty>> expression)
        {
            if (expression.Body is MemberExpression member)
            {
                return member.Member.Name;
            }
            throw new ArgumentException($"Lambda expression: {expression.Body} does not refer to a property.");
        }
    }

    internal static class ValidatorHelpers
    {
        public static readonly Dictionary<ValidationType, string> TypeDesc = new Dictionary<ValidationType, string>
        {
            { ValidationType.Select, "Select"},
            { ValidationType.OrderBy, "OrderBy"},
            { ValidationType.Search, "Search"},
            { ValidationType.Filter, "Filter"},
        };

        private static readonly Type[] _validNonPrimitive =
        {
            typeof(string), typeof(DateTime)
        };

        public static bool IsValidType(Type type)
        {
            return type.IsPrimitive || _validNonPrimitive.Contains(type);
        }
    }
}
