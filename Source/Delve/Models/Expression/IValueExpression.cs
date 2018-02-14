using System;
using System.Linq;

namespace Delve.Models.Expression
{
    internal interface IExpression<T> : IExpression
    {
        IQueryable<T> Apply(IQueryable<T> source, Func<IQueryable<T>, string, IQueryable<T>> customFunc = null);
    }
}
