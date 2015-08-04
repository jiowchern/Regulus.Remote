// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServerController.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the ServerController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System.Web.Mvc;

#endregion

namespace VGameWebApplication.Controllers
{
	[Authorize]
	public class ServerController : AsyncController
	{
		// GET: /Server/
		public ActionResult Dashboard(string server)
		{
			return View();
		}

		public ActionResult ServerState()
		{
			return PartialView(null);
		}
	}
}