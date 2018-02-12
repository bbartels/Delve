using System;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

using Delve.Models;
using Delve.Models.Validation;

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
                throw new ArgumentException($"Must use generic interface: {typeof(IResourceParameter<>)} " +
                                            $"of EntityType instead of: {typeof(IResourceParameter)}.");
            }

            var elementTypes = bindingContext.ModelType.GetGenericArguments();

            if (elementTypes.Length != 1)
            {
                throw new ArgumentException($"{nameof(IResourceParameter)} must have exactly one type argement.");
            }

            var parameterType = typeof(ResourceParameter<>).MakeGenericType(elementTypes[0]);
            var param = (IResourceParameter)Activator.CreateInstance(parameterType);

            var coll = HttpUtility.ParseQueryString(bindingContext.HttpContext.Request.QueryString.Value);
            var attributes = new[]
            {
                nameof(param.PageNumber).PascalToCamelCase(),
                nameof(param.PageSize).PascalToCamelCase(),
                "Filter",
                "OrderBy",
                "Select",
                "Expand"
            };

            if (int.TryParse(coll[attributes[0]], out int num)) { param.PageNumber = num; }
            if (int.TryParse(coll[attributes[1]], out int size)) { param.PageSize = size; }

            try
            {
                param.ApplyParameters(coll[attributes[2]], coll[attributes[3]], coll[attributes[4]], coll[attributes[5]]);
                var validatorType = typeof(IQueryValidator<>).MakeGenericType(elementTypes[0]);

                if (bindingContext.HttpContext.RequestServices.GetService(validatorType) is IQueryValidator validator)
                {
                    param.ValidateParameters(validator);
                }

                else
                {
                    throw new RuntimeException($"No service registered for QueryValidator: {validatorType}.");
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
