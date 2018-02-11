using System;
using Microsoft.Extensions.DependencyInjection;

using Delve.Models;

namespace Delve.AspNetCore
{
    public static class IMvcBuilderExtensions
    {
        public static IMvcBuilder AddDelve(this IMvcBuilder mvcBuilder, Action<DelveOptions> options = null)
        {
            var option = new DelveOptions();
            options?.Invoke(option);
            ResourceParameterOptions.ApplyConfig(option);

            mvcBuilder.AddMvcOptions(opt =>
                opt.ModelBinderProviders.Insert(0, new ResourceParamBinderProvider()));

            return mvcBuilder;
        }
    }
}
