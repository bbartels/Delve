using System;

using Delve.Models.Expressions;

namespace Delve.Models.Validation
{
    internal interface IInternalQueryValidator<T> : IQueryValidator
    {
        IValidationRule ValidateExpression(IExpression expression, ValidationType type);
        Type GetResultType(string key, ValidationType type);
        IQueryConfiguration<T> GetConfiguration();
    }

    public interface IQueryValidator { }

    public interface IQueryValidator<T> : IQueryValidator { }
}
