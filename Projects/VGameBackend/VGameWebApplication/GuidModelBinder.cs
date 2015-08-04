// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GuidModelBinder.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the GuidModelBinder type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System;
using System.Web.Mvc;

#endregion

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