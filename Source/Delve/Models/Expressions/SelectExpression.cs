using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Delve.Models.Expressions;

namespace Delve.Models
{
    internal class SelectExpression<T, TResult> : BaseExpression<T, TResult>, ISelectExpression<T>
    {
        public override string Query { get { return Key; } }

        public SelectExpression(string select)
        {
            Key = select;
        }

        public dynamic ApplySelect(IEnumerable<T> source)
        {
            return source;
        }
    }
}
