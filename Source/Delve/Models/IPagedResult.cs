using System.Collections.Generic;

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
    }
}
