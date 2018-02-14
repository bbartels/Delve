using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using Delve.Models.Validation;

namespace Delve.Models.Expression
{
    internal static class IExpressionExtensions
    {
        private static readonly Dictionary<ValidationType, Type> typeMap = new Dictionary<ValidationType, Type>
        {
            { ValidationType.Filter,  typeof(FilterExpression<,>) },
            { ValidationType.Expand,  typeof(ExpandExpression<,>) },
            { ValidationType.OrderBy, typeof(OrderByExpression<,>) },
            { ValidationType.Select,  typeof(SelectExpression<,>) }
        };

        public static ValidationType GetValidationType(Type type)
        {
            return typeMap.FirstOrDefault(x => x.Value == type.GetGenericTypeDefinition()).Key;
        }

        public static string GetQuery(this IEnumerable<IExpression> items)
        {
            return items.Select(x => x.Query).Aggregate((x, y) => $"{x},{y}");
        }

        public static List<IExpression<TEntity>> ParseQuery<TEntity>(this string query, 
                                    IInternalQueryValidator<TEntity> validator, ValidationType type)
        {
            var expressions = new List<IExpression<TEntity>>();

            if (query == null) { return expressions; }

            foreach (var subQuery in query.Split(','))
            {
                var key = QuerySanitizer.GetKey(type, subQuery);

                var expressionType = validator.GetResultType(key, type);
                expressions.Add((IExpression<TEntity>)MakeGenericType<TEntity>(typeMap[type], expressionType, subQuery));
            }

            return expressions;
        }

        private static IExpression MakeGenericType<TEntity>(Type expressionType, Type result, string query)
        {
            var type = expressionType.MakeGenericType(typeof(TEntity), result);
            return (IExpression)Activator.CreateInstance(type, query);
        }

        public static void ValidatePropertyType(this IExpression exp, string key, IEnumerable<string> values)
        {
            if (exp.PropertyType == typeof(string)) { return; }

            var currentValue = string.Empty;
            try
            {
                var converter = TypeDescriptor.GetConverter(exp.PropertyType);
                foreach (var value in values)
                {
                    currentValue = value;
                    converter?.ConvertFromString(value);
                }
            }
            catch (NotSupportedException)
            {
                throw new InvalidQueryException($"Value: {currentValue}: does not match " +
                                    $"datatype: {exp.PropertyType} of registered key: {key}");
            }
        }
    }
}
