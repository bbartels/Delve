using System.Collections.Generic;

using Delve.Models;

namespace Delve.Extensions
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<dynamic> ShapeData<T>(this IEnumerable<T> source, IResourceParameter param)
        {
            /*if (source == null) { throw new ArgumentNullException($"{nameof(source)}"); }

            var internalParam = (IInternalResourceParameter)param;
            if (internalParam.Select.Count != 0)
            {
                return source.AsQueryable().Select(SelectExpression.GetDynamicLinqQuery(internalParam.Select)).ToDynamicArray();
            }

            var propertyInfo = new List<PropertyInfo>(typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance));

            var objects = new List<ExpandoObject>();

            foreach (var item in source)
            {
                var modifiedObject = new ExpandoObject() as IDictionary<string, object>;

                foreach (var property in propertyInfo)
                {
                    modifiedObject.Add(property.Name, property.GetValue(item));
                }

                objects.Add((ExpandoObject)modifiedObject);
            }

            return objects;*/
            return null;
        }
    }
}
