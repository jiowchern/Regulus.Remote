// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RouteConfig.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the RouteConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System.Web.Mvc;
using System.Web.Routing;

#endregion

namespace VGameWebApplication
{
	public class RouteConfig
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.MapRoute("Default", "{controller}/{action}/{id}", new
			{
				controller = "Home", 
				action = "Index", 
				id = UrlParameter.Optional
			}
				);
		}
	}
}