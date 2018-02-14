using System;

using Delve.Models.Expressions;

namespace Delve.Models.Validation
{
    internal interface IValidationRule
    {
        ValidationType ValidationType { get; }
        IValidationRule ValidateExpression(IExpression exp);
        Type ResultType { get; }
    }
}
