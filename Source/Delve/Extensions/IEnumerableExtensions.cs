﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

using Delve.Models;

namespace Delve.Extensions
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<ExpandoObject> ShapeData<T>(this IEnumerable<T> source, IResourceParameter param)
        {
            if (source == null) { throw new ArgumentNullException($"{nameof(source)}"); }

            var propertyInfo = new List<PropertyInfo>();

            var internalParam = (IInternalResourceParameter)param;

            if (internalParam.Select.Count == 0)
            {
                propertyInfo.AddRange(typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance));
            }

            else
            {
                propertyInfo.AddRange(internalParam.Select.Select(field => 
                    typeof(T).GetProperty(field.PropertyExpression, 
                        BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase)));
            }

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

            return objects;
        }
    }
}
