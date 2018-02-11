using Delve.Models.Expression;

namespace Delve.Models.Validation
{
    internal interface IInternalQueryValidator
    {
        string ValidateExpression(INonValueExpression expression, ValidationType type);
        string ValidateValueExpression(IValueExpression expression, ValidationType type);
    }

    public interface IQueryValidator 
    {
    }

    public interface IQueryValidator<T> : IQueryValidator { }
}
