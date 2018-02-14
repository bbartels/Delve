using System;
using System.Collections.Generic;
using System.Linq;
using Delve.Models;

namespace Delve.Extensions
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<dynamic> ShapeData<T>(this IEnumerable<T> source, IResourceParameter param)
        {
            //if (source == null) { throw new ArgumentNullException($"{nameof(source)}"); }

            //var internalParam = (IInternalResourceParameter<T>)param;
            //if (internalParam.Select.Count != 0)
            //{
             //   return source.AsQueryable().Select(SelectExpression.GetDynamicLinqQuery(internalParam.Select)).ToDynamicArray();
            //}

            return null;
        }
    }
}
