
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Delve.Models.Expressions;

namespace Delve.Models
{
    internal class SelectExpression<T, TResult> : BaseExpression<T, TResult>
    {
        public override string Query { get { return Key; } }

        public SelectExpression(string select)
        {
            Key = select;
        }

        public override Func<T, (string, object)> GetPropertyMapping()
        {
            return (t) => (Key, Property.Compile()(t));
        }
    }
}
