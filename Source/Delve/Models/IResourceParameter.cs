using System.Collections.Generic;

namespace Delve.Models
{
    public interface IResourceParameter
    {
        int PageNumber { get; set; }
        int PageSize { get; set; }

        IList<FilterExpression> Filter { get; }
        IList<OrderByExpression> OrderBy { get; }
        IList<SelectExpression> Select { get; }

        void ApplyParameters(string filter, string orderBy, string select);

        Dictionary<string, string> GetPageHeader();
    }
}
