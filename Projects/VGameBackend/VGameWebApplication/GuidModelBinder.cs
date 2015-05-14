using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGameWebApplication
{
    class GuidModelBinder : System.Web.Mvc.IModelBinder
    {
        object System.Web.Mvc.IModelBinder.BindModel(System.Web.Mvc.ControllerContext controllerContext, System.Web.Mvc.ModelBindingContext bindingContext)
        {
            var parameter = bindingContext
            .ValueProvider
            .GetValue(bindingContext.ModelName);

            return Guid.Parse(parameter.AttemptedValue);
        }
    }
}
