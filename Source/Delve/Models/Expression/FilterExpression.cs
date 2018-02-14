using System;
using System.Linq;

using Delve.Models.Expression;

namespace Delve.Models
{
    internal class FilterExpression <T, TResult> : BaseExpression<T, TResult>
    {
        public QueryOperator Operator { get; }
        public string[] StringValues { get; }

        public override string Query
        {
            get { return $"{Key}{Operator.GetSymbol()}{StringValues.Aggregate((x, y) => $"{x},{y}")}"; }
        }

        public Func<TResult, TResult, bool> OperationExpression { get; private set; }
        public TResult[] Values { get; }


        public FilterExpression(string filter)
        {
            Operator = filter.GetQueryOperator();
            var (op1, op2) = filter.GetOperands();
            Key = op1;
            StringValues = op2.Split('|').Select(property => property.Trim()).ToArray();
            this.ValidatePropertyType(Key, StringValues);
            Values = StringValues.Select(x => (TResult)Convert.ChangeType(x, typeof(TResult))).ToArray();
            OperationExpression = ExpressionFactory<TResult>.RequestFunc(Operator);
        }

        public override IQueryable<T> Apply(IQueryable<T> source, 
            Func<IQueryable<T>, string, IQueryable<T>> customFunc = null)
        {
            return Values.Aggregate(source, (current, value) => current.Where(x => OperationExpression(Property(x), value)));
        }
    }
}
