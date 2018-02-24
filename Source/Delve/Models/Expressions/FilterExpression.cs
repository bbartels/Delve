using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Delve.Models.Validation;

namespace Delve.Models.Expressions
{
    internal class FilterExpression <T, TResult> : BaseExpression<T, TResult>
    {
        public QueryOperator Operator { get; }
        public string[] StringValues { get; }

        public override string Query
        {
            get
            {
                return $"{Key}{QuerySanitizer.GetFilterSymbol(Operator)}{StringValues.Aggregate((x, y) => $"{x},{y}")}";
            }
        }

        public Func<object, object, bool> OperationExpression { get; private set; }
        public IEnumerable<object> Values { get; }


        public FilterExpression(string filter)
        {
            Operator = QuerySanitizer.GetFilterOperator(filter);
            Key = QuerySanitizer.GetKey(ValidationType.Filter, filter);
            StringValues = QuerySanitizer.GetFilterValues(filter);

            var type = typeof(TResult).IsOrImplementsGenericIEnumerableExceptIsString()
                    ? typeof(TResult).GenericTypeArguments.FirstOrDefault()
                    : typeof(TResult);

            this.ValidatePropertyType(Key, StringValues);

            Values = StringValues.Select(x => Convert.ChangeType(x, type)).ToArray();
            OperationExpression = ExpressionFactory.RequestFunc(Operator, typeof(TResult));
        }

        public override IQueryable<T> ApplyFilter(IQueryable<T> source)
        {
            var compiledProp = Property.Compile();
            
            return source.Where(x => test1(x, compiledProp));
        }

        private bool test1(T test, Func<T, TResult> test2)
        {
            var any = false;
            foreach (var val in Values)
            {
                OperationExpression(1, new List<int> { 1, 2 });
                any = any || OperationExpression(val, test2(test));
            }

            return any;
        }
    }
}
