using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

using Delve.Models;

namespace Delve.AspNetCore
{
    public static class ResourceHelper
    {
        public static void AddPaginationHeader<T>(this Controller controller,
            IResourceParameter parameters, IPagedResult<T> result, IUrlHelper urlHelper)
        {
            var actionName = controller.ControllerContext.ActionDescriptor.AttributeRouteInfo.Name;
            var attributes = new[]
            {
                nameof(parameters.PageNumber).PascalToCamelCase(),
                nameof(parameters.PageSize).PascalToCamelCase()
            };

            var prevPage = result.HasPrevious ? urlHelper.Link(actionName, new Dictionary<string, string>
            {
                { attributes[0], (result.PageNumber - 1).ToString() },
                { attributes[1], result.PageSize.ToString() }
            }.AddRange(parameters.GetPageHeader())) : null;

            var nextPage = result.HasNext ? urlHelper.Link(actionName, new Dictionary<string, string>
            {
                { attributes[0], (result.PageNumber + 1).ToString() },
                { attributes[1], result.PageSize.ToString() }
            }.AddRange(parameters.GetPageHeader())) : null;

            var metaData = new
            {
                currentPage = result.PageNumber,
                pageSize = result.PageSize,
                totalPages = result.TotalPages,
                totalCount = result.TotalCount,
                previousPageLink = PercentEncodeReplace.Replace(prevPage),
                nextPageLink = PercentEncodeReplace.Replace(nextPage)
            };
            
            controller.Response.Headers.Add("X-Pagination", Newtonsoft.Json.JsonConvert.SerializeObject(metaData));
        }

        internal static string PascalToCamelCase(this string str)
        {
            return char.ToLower(str[0]) + str.Substring(1);
        }

        public static Dictionary<TKey, TValue> AddRange<TKey, TValue>(this Dictionary<TKey, TValue> d1, Dictionary<TKey, TValue> d2)
        {
            foreach (var d in d2)
            {
                d1.Add(d.Key, d.Value);
            }

            return d1;
        }
    }
}
