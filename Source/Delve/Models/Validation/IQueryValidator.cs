using System;

using Delve.Models.Expression;

namespace Delve.Models.Validation
{
    internal interface IInternalQueryValidator<T> : IQueryValidator
    {
        IValidationRule ValidateExpression(IExpression expression, ValidationType type);
        Type GetResultType(string key, ValidationType type);
    }

    public interface IQueryValidator 
    {
    }

    public interface IQueryValidator<T> : IQueryValidator { }
}
