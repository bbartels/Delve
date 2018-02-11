using System.Collections.Generic;
using System.Dynamic;

namespace Delve.Models
{
    public interface IPagedResult<T> : IList<T>
    {
        int PageNumber { get; }
        int PageSize { get; }

        int TotalPages { get; }
        int TotalCount { get; }

        bool HasPrevious { get; }
        bool HasNext { get; }

        IEnumerable<ExpandoObject> Shape(IResourceParameter param);
    }
}
