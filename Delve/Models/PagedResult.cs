using System.Collections.Generic;
using System.Dynamic;

using Delve.Extensions;

namespace Delve.Models
{
    public class PagedResult<T> : List<T>, IPagedResult<T>
    {
        public int PageNumber { get; private set; }
        public int TotalPages { get; private set; }

        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }

        public bool HasPrevious
        {
            get { return PageNumber > 1; }
        }

        public bool HasNext
        {
            get { return PageNumber < TotalPages; }
        }

        public PagedResult(IEnumerable<T> items, int pageNumber, int pageSize, int count)
        {
            AddRange(items);
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalCount = count;
            TotalPages = (count + pageSize - 1) / pageSize;
        }

        public IEnumerable<ExpandoObject> Shape(IResourceParameter param)
        {
            return this.ShapeData(param);
        }
    }
}
