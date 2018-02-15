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

            //Adds UrlHelper services to mvc app
            mvcBuilder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            mvcBuilder.Services.AddScoped<IUrlHelper, UrlHelper>(factory => 
                            new UrlHelper(factory.GetService<IActionContextAccessor>().ActionContext));

            //Adds IResourceParameter<T> ModelBinding and ModelState validator
            mvcBuilder.AddMvcOptions(opt =>
            {
                opt.ModelBinderProviders.Insert(0, new ResourceParamBinderProvider());
                opt.Filters.Add(new ModelStateValidator());
            });

            //Ignores CiruclarReference checking in json.net
            mvcBuilder.AddJsonOptions(opt => {
                opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });

            return mvcBuilder;
        }
    }
}
