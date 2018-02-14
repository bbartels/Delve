using System.Collections.Generic;

using Delve.Models.Validation;

namespace Delve.Models
{
    public interface IResourceParameter
    {
        int PageNumber { get; set; }
        int PageSize { get; set; }

        void ApplyParameters(IQueryValidator validator, string filter, string orderBy, string select, string expand);

        Dictionary<string, string> GetPageHeader();
    }

    public interface IResourceParameter<T> : IResourceParameter { }
}
