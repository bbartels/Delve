using System.Collections.Generic;
using System.Linq;

using Delve.AspNetCore;

namespace Delve.Models
{
    public class ResourceParameter<T> : IResourceParameter
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

        public IList<FilterExpression> Filter { get; internal set; }
        public IList<OrderByExpression> OrderBy { get; internal set; }
        public IList<SelectExpression> Select { get; internal set; }

        public void ApplyParameters(string filter, string orderBy, string select)
        {
            Filter = FilterExpression.ParseExpression(filter).ToList();
            OrderBy = OrderByExpression.ParseExpression(orderBy).ToList();
            Select = SelectExpression.ParseExpression(select).ToList();
        }

        public Dictionary<string, string> GetPageHeader()
        {
            var dict = new Dictionary<string, string>();
            if (Filter.Any())   { dict.Add("filter", FilterExpression.GetQuery(Filter)); }
            if (OrderBy.Any())  { dict.Add("orderBy", OrderByExpression.GetQuery(OrderBy)); }
            if (Select.Any())   { dict.Add("select", SelectExpression.GetSelectQuery(Select)); }

            return dict;
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
