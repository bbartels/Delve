using Delve.Models.Validation;

namespace Delve.Models.Expression
{
    internal interface IExpression
    {
        string PropertyExpression { get; }
        string Key { get; }
        string Query { get; }

        void ValidateExpression(IQueryValidator validator);
    }
}
