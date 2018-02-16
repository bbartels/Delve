using System;
using System.Linq.Expressions;

using Delve.Models.Validation;
using Delve.Tests.Models;

namespace Delve.Tests
{
    internal class TestValidator : AbstractQueryValidator<User>
    {
        public void CanSelectTest<T>(string key, Expression<Func<User, T>> exp)
        {
            CanSelect(key, exp);
        }

        public void CanFilterTest<T>(string key, Expression<Func<User, T>> exp)
        {
            CanFilter(key, exp);
        }

        public void CanOrderTest<T>(string key, Expression<Func<User, T>> exp)
        {
            CanOrder(key, exp);
        }

        public void CanExpandTest<T>(string key, Expression<Func<User, T>> exp) where T : class
        {
            CanExpand(exp);
        }

        public void AllowAllTest<T>(string key, Expression<Func<User, T>> exp)
        {
            AllowAll(key, exp);
        }
    }
}
