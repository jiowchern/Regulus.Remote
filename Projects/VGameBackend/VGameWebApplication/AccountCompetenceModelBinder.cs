using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Regulus.Extension;

namespace VGameWebApplication
{
    class AccountCompetenceModelBinder : System.Web.Mvc.IModelBinder
    {
        object System.Web.Mvc.IModelBinder.BindModel(System.Web.Mvc.ControllerContext controllerContext, System.Web.Mvc.ModelBindingContext bindingContext)
        {
            var flags = new Regulus.CustomType.Flag<VGame.Project.FishHunter.Data.Account.COMPETENCE>();
            foreach (var t in EnumHelper.GetFlags<VGame.Project.FishHunter.Data.Account.COMPETENCE>())
            {
                var val = string.Format("{0}[{1}]", bindingContext.ModelName, t);
                var parameter = bindingContext.ValueProvider.GetValue(val);
                var check = (bool)parameter.ConvertTo(typeof(bool));
                flags[t] = check;                 
            }



            return flags;
                 
        }
    }
}
