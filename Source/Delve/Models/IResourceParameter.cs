using System.Collections.Generic;

using Delve.Models.Validation;

namespace Delve.Models
{
    public interface IResourceParameter
    {
        int PageNumber { get; set; }
        int PageSize { get; set; }

        void ApplyParameters(string filter, string orderBy, string select);

        Dictionary<string, string> GetPageHeader();

        void ValidateParameters(IQueryValidator validator);
    }

    public interface IResourceParameter<T> : IResourceParameter { }
}
