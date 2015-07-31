// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HomeController.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the HomeController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

using VGameWebApplication.Models;
using VGameWebApplication.Storage;

#endregion

namespace VGameWebApplication.Controllers
{
	public class HomeController : AsyncController
	{
		public ActionResult Logout()
		{
			FormsAuthentication.SignOut();
			var authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];

			var authTicket = FormsAuthentication.Decrypt(authCookie.Value);
			Service.Destroy((Guid)HttpContext.Items["StorageId"]);

			return RedirectToAction("Verify");
		}

		// GET: /Home/        
		public ActionResult Index()
		{
			if (User.Identity.IsAuthenticated == false)
			{
				return RedirectToAction("Verify");
			}

			return View();
		}

		// GET: /Home/        
		public ActionResult Verify()
		{
			return View();
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public async Task<ActionResult> Verify(string user, string password)
		{
			var id = Service.Verify(user, password);

			if (id != Guid.Empty)
			{
				var ticket = new FormsAuthenticationTicket
					(1
						, user, 
						DateTime.Now, 
						DateTime.Now.AddSeconds(300), 
						false, 
						id.ToString());

				var encTicket = FormsAuthentication.Encrypt(ticket);
				var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
				Response.Cookies.Add(cookie);
				return RedirectToAction("Index", "Home");
			}

			return RedirectToAction("Verify");
		}

		[Authorize]
		public ActionResult Functions()
		{
			var service = Service.Create(HttpContext.Items["StorageId"]);

			var accountFunctions = new AccountFunctions();

			accountFunctions.AccountManager = service.AccountManager != null;
			accountFunctions.AccountFinder = service.AccountFinder != null;
			return PartialView(accountFunctions);
		}

		public ActionResult Download()
		{
			var fileBytes = System.IO.File.ReadAllBytes(@"c:\Ftp\FishHunter.apk");
			var fileName = "FishHunter.apk";
			return File(fileBytes, MediaTypeNames.Application.Octet, fileName);
		}
	}
}