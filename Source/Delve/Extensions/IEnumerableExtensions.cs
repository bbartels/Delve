using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using Delve.Models;

namespace Delve.Extensions
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<object> ShapeData<TEntity>(this IEnumerable<TEntity> source, IResourceParameter param)
        {
            if (source == null) { throw new ArgumentNullException($"{nameof(source)}"); }

            var internalParam = (IInternalResourceParameter<TEntity>)param;
            return internalParam.ApplySelect(source);
        }
    }
}
