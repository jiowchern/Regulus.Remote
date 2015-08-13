using System.Web.Mvc;

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
