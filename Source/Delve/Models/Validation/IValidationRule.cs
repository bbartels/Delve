using Delve.Models.Expression;

namespace Delve.Models.Validation
{
    internal interface IValidationRule
    {
        ValidationType ValidationType { get; }

        string ValidateExpression(INonValueExpression expression);
        string ValidateValueExpression(IValueExpression expression);
    }
}
