using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Threading.Tasks;

namespace MyFaceApi.IdentityServer.Application.Helpers
{
	public class ArrayModelBinder : IModelBinder
	{
		Task IModelBinder.BindModelAsync(ModelBindingContext bindingContext)
		{
			if (!bindingContext.ModelMetadata.IsEnumerableType)
			{
				bindingContext.Result = ModelBindingResult.Failed();
				return Task.CompletedTask;
			}

			var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName).ToString();

			if (string.IsNullOrWhiteSpace(value))
			{
				bindingContext.Result = ModelBindingResult.Success(null);
				return Task.CompletedTask;
			}

			var values = value.Split(",");

			var typedValues = Array.CreateInstance(typeof(string), values.Length);
			values.CopyTo(typedValues, 0);
			bindingContext.Model = typedValues;
			bindingContext.Result = ModelBindingResult.Success(bindingContext.Model);
			return Task.CompletedTask;
		}
	}
}
