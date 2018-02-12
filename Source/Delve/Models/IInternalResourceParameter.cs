using System.Collections.Generic;
using Delve.Models.Expression;

namespace Delve.Models
{
    internal interface IInternalResourceParameter
    {
        IList<IValueExpression> Filter { get; }
        IList<INonValueExpression> OrderBy { get; }
        IList<INonValueExpression> Select { get; }
    }
}
