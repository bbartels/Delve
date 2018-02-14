using System;
using System.Collections.Generic;
using System.Linq;

using Delve.Models.Expression;
using Delve.Models.Validation;

namespace Delve.Models
{
    internal class ResourceParameter<T> : IResourceParameter<T>, IInternalResourceParameter<T>
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

        IQueryable<T> IInternalResourceParameter<T>.ApplyOrderBy(IQueryable<T> source)
        {
            OrderBy.ForEach(o => source = o.Apply(source));
            return source;
        }

        IQueryable<T> IInternalResourceParameter<T>.ApplyExpand(IQueryable<T> source, Func<IQueryable<T>, string, IQueryable<T>> Include)
        {
            Expand.ForEach(e => source = e.Apply(source, Include));
            return source;
        }

        IQueryable<T> IInternalResourceParameter<T>.ApplyFilters(IQueryable<T> source)
        {
            Filter.ForEach(f => source = f.Apply(source));
            return source;
        }

        internal List<IExpression<T>> Filter { get; private set; }
        internal List<IExpression<T>> OrderBy { get; private set; }
        internal List<IExpression<T>> Select { get; private set; }
        internal List<IExpression<T>> Expand { get; private set; }

        public void ApplyParameters(IQueryValidator validator, string filter, string orderBy, string select, string expand)
        {
            var internalValidator = (IInternalQueryValidator<T>)validator;
            Filter = filter.ParseQuery(internalValidator, ValidationType.Filter);
            OrderBy = orderBy.ParseQuery(internalValidator, ValidationType.OrderBy);
            Select = select.ParseQuery(internalValidator, ValidationType.Select);
            Expand = expand.ParseQuery(internalValidator, ValidationType.Expand);

            foreach (var expression in Filter.Concat(OrderBy.Concat(Select.Concat(Expand))))
            {
                expression.ValidateExpression(validator);
            }
        }

        public Dictionary<string, string> GetPageHeader()
        {
            var dict = new Dictionary<string, string>();
            if (Filter.Any())   { dict.Add("filter", Filter.GetQuery()); }
            if (OrderBy.Any())  { dict.Add("orderBy", OrderBy.GetQuery()); }
            if (Select.Any())   { dict.Add("select", Select.GetQuery()); }
            if (Expand.Any())   { dict.Add("expand", Expand.GetQuery());}

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
