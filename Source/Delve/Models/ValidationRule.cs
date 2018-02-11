using System;
using System.Collections.Generic;

namespace Delve.Models
{
    internal class ValidationRule : HashSet<ValidationType>
    {
        public string PropertyName { get; private set; }
        public Type PropertyType { get; private set; }

        public ValidationRule(ValidationType type, string propertyName, Type propertyType)
        {
            Add(type);
            PropertyName = propertyName;
            PropertyType = propertyType;
        }
    }

    internal enum ValidationType
    {
        Filter,
        OrderBy,
        Select,
        Search
    }
}
