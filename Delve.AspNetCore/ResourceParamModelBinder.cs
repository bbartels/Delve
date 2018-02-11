using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

using Delve.Models;

namespace Delve.AspNetCore
{
    public class ResourceParamBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return typeof(IResourceParameter).IsAssignableFrom(context.Metadata.ModelType) ? 
                new BinderTypeModelBinder(typeof(ResourceParamModelBinder)) : null;
        }
    }

    public class ResourceParamModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext.ModelType == typeof(IResourceParameter))
            {
                throw new ArgumentException($"Must user concrete implementation: {typeof(ResourceParameter<>)} " +
                                            $"of EntityTypeinstead of: {typeof(IResourceParameter)}.");
            }

            var param = (IResourceParameter)Activator.CreateInstance(bindingContext.ModelType);

            var coll = HttpUtility.ParseQueryString(bindingContext.HttpContext.Request.QueryString.Value);
            var attributes = new[]
            {
                nameof(param.PageNumber).PascalToCamelCase(),
                nameof(param.PageSize).PascalToCamelCase(),
                nameof(param.Filter).PascalToCamelCase(),
                nameof(param.OrderBy).PascalToCamelCase(),
                nameof(param.Select).PascalToCamelCase()
            };

            if (int.TryParse(coll[attributes[0]], out int num)) { param.PageNumber = num; }
            if (int.TryParse(coll[attributes[1]], out int size)) { param.PageSize = size; }

            try
            {
                param.ApplyParameters(coll[attributes[2]], coll[attributes[3]], coll[attributes[4]]);
                var elementType = bindingContext.ModelType.GetGenericArguments().FirstOrDefault();
                var validatorType = typeof(IQueryValidator<>).MakeGenericType(elementType);

                if (bindingContext.HttpContext.RequestServices.GetService(validatorType) is IQueryValidator validator)
                {
                    validator.Validate(param);
                }

                else
                {
                    bindingContext.Result = ModelBindingResult.Failed();
                    return Task.CompletedTask;
                }
            }
            catch (InvalidQueryException e)
            {
                bindingContext.ModelState.AddModelError("InvalidQueryString", e.Message);
                bindingContext.Result = ModelBindingResult.Failed();
                return Task.CompletedTask;
            }

            bindingContext.Result = ModelBindingResult.Success(param);
            return Task.CompletedTask;
        }
    }
}
