// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Global.asax.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the MvcApplication type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System;
using System.IO;
using System.Reflection;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

using Regulus.CustomType;

using VGame.Project.FishHunter.Common;
using VGame.Project.FishHunter.Common.Datas;

using VGameWebApplication.Storage;

#endregion

namespace VGameWebApplication
{
	public class MvcApplication : HttpApplication
	{
		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();
			RouteConfig.RegisterRoutes(RouteTable.Routes);

			var asm = Assembly.Load("App_Code/AssemblyInfo.cs");
			Application.Add("Assembly", asm);

			Application.Add("BuildDateTime", 
				new FileInfo(Assembly.GetExecutingAssembly().Location).LastWriteTime.ToShortDateString());

			ModelBinders.Binders.Add(typeof (Guid), new GuidModelBinder());

			ModelBinders.Binders.Add(typeof (Flag<Account.COMPETENCE>), new AccountCompetenceModelBinder());
		}

		protected void Application_PostAuthenticateRequest(object sender, EventArgs e)
		{
			var authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
			if (authCookie != null)
			{
				var authTicket = FormsAuthentication.Decrypt(authCookie.Value);
				Guid id;
				if (Guid.TryParse(authTicket.UserData, out id) == false || Service.Create(id) == null)
				{
					Service.Destroy(id);
					HttpContext.Current.User = new GenericPrincipal(new GenericIdentity(string.Empty), new string[0]);
				}
				else
				{
					HttpContext.Current.Items["StorageId"] = id;
				}
			}
		}
	}
}