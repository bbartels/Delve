using System.Collections.Generic;
using System.Linq;

using Delve.Models.Expression;
using Delve.Models.Validation;

namespace Delve.Models
{
    internal class ResourceParameter<T> : IResourceParameter<T>
    {
        private int _pageNumber = 1;
        public int PageNumber
        {
            get { return _pageNumber; }
            set
            {
                if (value >= 1) { _pageNumber = value; }
            }
        }

        private int _pageSize = ResourceParameterOptions.DefaultPageSize;
        public int PageSize
        {
            get { return _pageSize; }
            set 
            {
                if (value > ResourceParameterOptions.MaxPageSize &&
                    value > 0)
                {
                    _pageSize = value;
                }
            }
        }

        public IList<IValueExpression> Filter { get; internal set; }
        public IList<INonValueExpression> OrderBy { get; internal set; }
        public IList<INonValueExpression> Select { get; internal set; }

        (string query, object[] values) IResourceParameter.Filter
        {
            get { return FilterExpression.GetDynamicLinqQuery(Filter); }
        }

        string IResourceParameter.OrderBy
        {
            get
            {
                throw new System.NotImplementedException();
            }
        }

        string IResourceParameter.Select
        {
            get
            {
                throw new System.NotImplementedException();
            }
        }

        public void ApplyParameters(string filter, string orderBy, string select)
        {
            Filter = filter.ParseQuery<IValueExpression>(x => new FilterExpression(x));
            OrderBy = orderBy.ParseQuery<INonValueExpression>(x => new OrderByExpression(x));
            Select = select.ParseQuery<INonValueExpression>(x => new SelectExpression(x));
        }

        public Dictionary<string, string> GetPageHeader()
        {
            var dict = new Dictionary<string, string>();
            if (Filter.Any())   { dict.Add("filter", Filter.GetQuery()); }
            if (OrderBy.Any())  { dict.Add("orderBy", OrderBy.GetQuery()); }
            if (Select.Any())   { dict.Add("select", Select.GetQuery()); }

            return dict;
        }

        public void ValidateParameters(IQueryValidator validator)
        {
            foreach (var expression in OrderBy.Concat(Select))
            {
                expression.ValidateExpression(validator);
            }

            foreach (var expression in Filter)
            {
                expression.ValidateExpression(validator);
            }
        }
    }

    public static class ResourceParameterOptions
    {
        public static int MaxPageSize { get; set; } = 25;
        public static int DefaultPageSize { get; set; } = 10;

        public static void ApplyConfig(DelveOptions options)
        {
            MaxPageSize = options.MaxPageSize;
            DefaultPageSize = options.DefaultPageSize;
        }
    }
}
