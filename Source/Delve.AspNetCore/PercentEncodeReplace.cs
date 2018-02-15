using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Delve.AspNetCore
{
    public static class PercentEncodeReplace
    {
        private static readonly IDictionary<string, string> percentEncodeMaps = new Dictionary<string, string>
        {
            { "%20", " "},
            { "%21", "!" },
            { "%24", "$" },
            { "%2A", "*" },
            { "%3C", "<" },
            { "%3D", "=" },
            { "%3E", ">" },
            { "%3F", "?" },
            { "%5E", "^" },
        };

        public static string Replace(string url)
        {
            if (url == null) { return null; }

            return percentEncodeMaps.Aggregate(url, (current, map) 
                                => current.Replace(map.Key, map.Value));
        }
    }
}
