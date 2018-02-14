using System;

using Delve.Models.Validation;

namespace Delve.Models.Expression
{
    internal interface IExpression
    {
        string Key { get; }
        string Query { get; }
        Type PropertyType { get; }

        void ValidateExpression(IQueryValidator validator);
    }
}
