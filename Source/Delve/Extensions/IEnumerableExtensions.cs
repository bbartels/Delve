using System;
using System.Collections.Generic;

namespace Delve.Models
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
