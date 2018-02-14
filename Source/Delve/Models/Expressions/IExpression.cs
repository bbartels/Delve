using System;
using System.Linq;
using Delve.Models.Validation;

namespace Delve.Models.Expressions
{
    internal interface IExpression
    {
        string Key { get; }
        string Query { get; }
        Type PropertyType { get; }

        void ValidateExpression(IQueryValidator validator);
    }

    internal interface IExpression<T> : IExpression
    {
        IQueryable<T> Apply(IQueryable<T> source, Func<IQueryable<T>, string, IQueryable<T>> customFunc = null);
    }
}
