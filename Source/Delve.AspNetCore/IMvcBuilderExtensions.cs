using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
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

            mvcBuilder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            mvcBuilder.Services.AddScoped<IUrlHelper, UrlHelper>(factory => 
                            new UrlHelper(factory.GetService<IActionContextAccessor>().ActionContext));

            mvcBuilder.AddMvcOptions(opt =>
            {
                opt.ModelBinderProviders.Insert(0, new ResourceParamBinderProvider());
                opt.Filters.Add(new ModelStateValidator());
            });

            return mvcBuilder;
        }
    }
}
