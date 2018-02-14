using System.Collections;
using System.Collections.Generic;

namespace Delve.Models.Expression
{
    internal interface ISelectExpression<T>
    {
        dynamic ApplySelect(IEnumerable<T> source);
    }
}
