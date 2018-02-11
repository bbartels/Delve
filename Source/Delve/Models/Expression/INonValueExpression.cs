namespace Delve.Models.Expression
{
    internal interface INonValueExpression : IExpression
    {
        string GetDynamicLinqQuery();
    }
}
