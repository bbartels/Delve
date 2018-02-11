namespace Delve.Models.Expression
{
    internal interface IValueExpression : IExpression
    {
        string[] Values { get; }
        (string query, string[] values) GetDynamicLinqQuery(int startIndex = 0);
    }
}
