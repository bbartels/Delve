using System;
using System.Collections.Generic;
using System.Text;

namespace Delve.Models
{
    internal interface ISelectConfiguration<T>
    {
        Func<T, (string, object)> GetPropertyMapping();
    }
}
