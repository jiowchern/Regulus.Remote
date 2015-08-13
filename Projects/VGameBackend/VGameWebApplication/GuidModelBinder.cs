using System;
using System.Web.Mvc;

namespace VGameWebApplication
{
	internal class GuidModelBinder : IModelBinder
	{
		object IModelBinder.BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
		{
			var parameter = bindingContext
				.ValueProvider
				.GetValue(bindingContext.ModelName);

			return Guid.Parse(parameter.AttemptedValue);
		}
	}
}
