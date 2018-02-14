using System;
using System.Linq;
using System.Linq.Expressions;

using Delve.Models.Expressions;
using Delve.Models.Validation;

namespace Delve.Models
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

        public Expression<Func<TResult, TResult, bool>> OperationExpression { get; private set; }
        public TResult[] Values { get; }


        public FilterExpression(string filter)
        {
            Operator = QuerySanitizer.GetFilterOperator(filter);
            Key = QuerySanitizer.GetKey(ValidationType.Filter, filter);
            StringValues = QuerySanitizer.GetFilterValues(filter);
            this.ValidatePropertyType(Key, StringValues);

            Values = StringValues.Select(x => (TResult)Convert.ChangeType(x, typeof(TResult))).ToArray();
            OperationExpression = ExpressionFactory<TResult>.RequestFunc(Operator);
        }

        public override IQueryable<T> ApplyFilter(IQueryable<T> source)
        {
            var compiledExp = OperationExpression.Compile();
            var compiledProp = Property.Compile();

            return source.Where(x => Values.Any(v => compiledExp(v, compiledProp(x))));
        }
    }
}
