using System.Collections.Generic;

using Delve.Models.Validation;

namespace Delve.Models
{
    public interface IResourceParameter
    {
        int PageNumber { get; set; }
        int PageSize { get; set; }

        (string query, object[] values) Filter { get; }
        string OrderBy { get; }
        string Select { get; }


        void ApplyParameters(string filter, string orderBy, string select);

        Dictionary<string, string> GetPageHeader();

        void ValidateParameters(IQueryValidator validator);
    }

    public interface IResourceParameter<T> : IResourceParameter { }
}
