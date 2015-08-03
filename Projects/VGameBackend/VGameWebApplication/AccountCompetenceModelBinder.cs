// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AccountCompetenceModelBinder.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the AccountCompetenceModelBinder type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System.Web.Mvc;

using Regulus.CustomType;
using Regulus.Utility;

using VGame.Project.FishHunter.Common;
using VGame.Project.FishHunter.Common.Datas;

#endregion

namespace VGameWebApplication
{
	internal class AccountCompetenceModelBinder : IModelBinder
	{
		object IModelBinder.BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
		{
			var flags = new Flag<Account.COMPETENCE>();
			foreach (var t in EnumHelper.GetEnums<Account.COMPETENCE>())
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