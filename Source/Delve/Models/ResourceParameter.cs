using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

using Delve.Models.Expressions;
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
                if (value <= ResourceParameterOptions.MaxPageSize &&
                    value > 0)
                {
                    _pageSize = value;
                }
            }
        }

        private IQueryConfiguration<T> _configuration;
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
            _configuration = internalValidator.GetConfiguration();

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

        IQueryable<T> IInternalResourceParameter<T>.ApplyOrderBy(IQueryable<T> source)
        {
            if (OrderBy.Count == 0)
            {
                return _configuration.ApplyDefaultSorts(source);
            }

            bool thenBy = false;

            foreach (var sort in OrderBy)
            {
                source = sort.ApplySort(source, thenBy);
                thenBy = true;
            }

            return source;
        }

        IQueryable<T> IInternalResourceParameter<T>.ApplyExpand(IQueryable<T> source, Func<IQueryable<T>, string, IQueryable<T>> Include)
        {
            Expand.ForEach(e => source = e.ApplyExpand(source, Include));
            return source;
        }

        IList<object> IInternalResourceParameter<T>.ApplySelect(IEnumerable<T> source)
        {
            if (Select.Count == 0)
            {
                return _configuration.ApplyDefaultSelects(source);
            }

            var test = Select.Select(x => x.GetPropertyMapping());

            object applyFunc(T user, IEnumerable<Func<T, (string, object)>> func)
            {
                var t = new ExpandoObject() as IDictionary<string, object>;
                foreach (var f in func)
                {
                    var elements = f(user);
                    t.Add(elements.Item1, elements.Item2);
                }

                return (ExpandoObject)t;
            }

            return source.Select(x => applyFunc(x, test)).ToList();
        }

        IQueryable<T> IInternalResourceParameter<T>.ApplyFilters(IQueryable<T> source)
        {
            if (Filter.Count == 0)
            {
                return _configuration.ApplyDefaultFilters(source);
            }

            Filter.ForEach(f => source = f.ApplyFilter(source));
            return source;
        }
    }

    public static class ResourceParameterOptions
    {
        public static int MaxPageSize { get; set; } = 25;
        public static int DefaultPageSize { get; set; } = 10;
        public static bool IgnoreNullOnSerialization { get; set; } = true;
        public static bool OmitHostOnPaginationLinks { get; set; } = false;

        public static void ApplyConfig(DelveOptions options)
        {
            MaxPageSize = options.MaxPageSize;
            DefaultPageSize = options.DefaultPageSize;
            IgnoreNullOnSerialization = options.IgnoreNullOnSerilazation;
            OmitHostOnPaginationLinks = options.OmitHostOnPaginationLinks;
        }
    }
}
