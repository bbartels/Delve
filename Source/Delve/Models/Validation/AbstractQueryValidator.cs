using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

using Delve.Models.Expressions;

namespace Delve.Models.Validation
{
    public class AbstractQueryValidator<T> : IQueryValidator<T>, IInternalQueryValidator<T>
    {
        private readonly IDictionary<string, ValidationRules> _validationRules = 
                new Dictionary<string, ValidationRules>(StringComparer.CurrentCultureIgnoreCase);

        private readonly IQueryConfiguration<T> _defaultConfig = new QueryConfiguration<T>();

        private void AddRule<TResult>(string key, IValidationRule rule)
        {
            ValidatorHelpers.CheckTypeValid(typeof(TResult), rule.ValidationType, key);

            if (!Regex.IsMatch(key, @"^[a-zA-Z0-9_.]+$"))
            {
                throw new InvalidValidationBuilderException($"Key: \"{key}\" must only contain numbers, " +
                                                            $"letters, dots and underscores.");
            }

            if (!_validationRules.ContainsKey(key))
            {
                _validationRules.Add(key, new ValidationRules(rule, key)); 
            }

            else
            {
                _validationRules[key].AddRule(rule, key);
            }
        }

        protected void AddDefaultFilter(Expression<Func<T, bool>> expression)
        {
            _defaultConfig.AddDefaultFilter(expression);
        }

        protected void AddDefaultSort<TResult>(Expression<Func<T, TResult>> exp, bool descending)
        {
            _defaultConfig.AddDefaultSort(exp, descending);
        }

        protected void AddDefaultSelect<TResult>(string key, Expression<Func<T, TResult>> exp)
        {
            _defaultConfig.AddDefaultSelect(key, exp);
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

        protected void CanExpand<TResult>(Expression<Func<T, TResult>> exp) where TResult : class
        {
            AddRule<TResult>(ValidatorHelpers.GetExpandString(exp.ToString()), 
                                            new ValidationRule<T, TResult>(exp, ValidationType.Expand));
        }

        protected void AllowAll<TResult>(string key, Expression<Func<T, TResult>> exp)
        {
            CanFilter(key, exp);
            CanSelect(key, exp);
            CanOrder(key, exp);
        }

        private void ValidateKey(string key, ValidationType type)
        {
            if (type != ValidationType.Expand)
            {
                if (!_validationRules.ContainsKey(key))
                {
                    throw new InvalidQueryException($"Invalid \"{type}\" " +
                                                    $"propertykey: \"{key}\" in query.");
                }
            }

            else
            {
                if (GetExpandKey(key) == null) { throw new InvalidQueryException($"Key \"{key}\" is not defined."); }
            }
        }

        IValidationRule IInternalQueryValidator<T>.ValidateExpression(IExpression expression, ValidationType type)
        {
            ValidateKey(expression.Key, type);
            return _validationRules[type == ValidationType.Expand ? GetExpandKey(expression.Key) : expression.Key]
                                   .ValidateExpression(type, expression);
        }

        Type IInternalQueryValidator<T>.GetResultType(string key, ValidationType type)
        {
            ValidateKey(key, type);
            return _validationRules[type == ValidationType.Expand ? GetExpandKey(key) : key].GetResultType(type, key);
        }

        IQueryConfiguration<T> IInternalQueryValidator<T>.GetConfiguration()
        {
            return _defaultConfig;
        }

        private string GetExpandKey(string key)
        {
            if (key.EndsWith(".")) { return null; }
            return _validationRules
                .Where(pair => pair.Key.IndexOf(key, StringComparison.CurrentCultureIgnoreCase) == 0).Select(x => x.Key)
                .FirstOrDefault();
        }
    }

    internal static class ValidatorHelpers
    {
        private static readonly Type[] _validNonPrimitive =
        {
            typeof(string), typeof(DateTime), typeof(TimeSpan)
        };

        public static void CheckTypeValid(Type type, ValidationType valType, string key)
        {
            switch (valType)
            {
                case ValidationType.Select: { } break;
                case ValidationType.OrderBy: { } break;

                case ValidationType.Filter:
                {
                    if (!type.IsPrimitive && !_validNonPrimitive.Contains(type) && !typeof(IEnumerable).IsAssignableFrom(type))
                    {
                        throw new InvalidValidationBuilderException
                            ($"Registered filter property: \"{key}\" can not be of type \"{type}\".");
                    }
                } break;

                case ValidationType.Expand:
                {
                    if (type.IsValueType)
                    {
                        throw new InvalidValidationBuilderException
                            ($"Registered property: \"{key}\" can not be of type: \"{type}\". Expand is limited to classes.");
                    }
                } break;

                default:
                {
                    throw new ArgumentOutOfRangeException(nameof(valType), valType, null);
                }
            }
        }

        public static string GetExpandString(string expression)
        {
            var split = expression.Split(new[] { "=>" }, StringSplitOptions.None).ToList();

            var properties = new List<string>();

            foreach(var str in split)
            {
	           if(!str.Contains('.')) { continue; }
                var propSplit = str.Split('.');
                for(int i = 1; i < propSplit.Length; i++)
                {
                     if (!propSplit[i].StartsWith("Select"))
                     {
                          properties.Add(propSplit[i].Trim(')').Trim('('));
                     }
                }
            }

            return properties.Aggregate((x, y) => x + '.' + y);
        }
    }
}
