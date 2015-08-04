// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AccountController.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the AccountController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System;
using System.Threading.Tasks;
using System.Web.Mvc;

using Regulus.Net45;

using VGame.Project.FishHunter.Common;
using VGame.Project.FishHunter.Common.Data;


using VGameWebApplication.Models;
using VGameWebApplication.Storage;

#endregion

namespace VGameWebApplication.Controllers
{
	[Authorize]
	public class AccountController : AsyncController
	{
		private Account[] _Accounts;

		// GET: /Account/
		public ActionResult Index()
		{
			var model = Service.Create(HttpContext.Items["StorageId"]);
			var accounts = model.AccountManager.QueryAllAccount().WaitResult();
			model.Release();
			return View(accounts);
		}

		private void _GetAccounts(Account[] obj)
		{
			_Accounts = obj;
		}

		public ActionResult Add()
		{
			return View(new Account());
		}

		public async Task<ActionResult> AddHandle(Account account)
		{
			var model = Service.Create(HttpContext.Items["StorageId"]);

			account.Id = Guid.NewGuid();
			var result = await model.AccountManager.Create(account).ToTask();

			model.Release();
			return RedirectToAction("Index");
		}

		public async Task<ActionResult> Edit(string accountid)
		{
			Guid accountId;
			if (Guid.TryParse(accountid, out accountId) == false)
			{
				return RedirectToAction("Index");
			}

			var model = Service.Create(HttpContext.Items["StorageId"]);
			var result = await model.AccountFinder.FindAccountById(accountId).ToTask();
			model.Release();
			if (result.Id != Guid.Empty)
			{
				var updateAccount = new UpdateAccount();
				updateAccount.TheAccount = result;
				return View(updateAccount);
			}

			return RedirectToAction("Index");
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public async Task<ActionResult> Edit(UpdateAccount account)
		{
			if (ModelState.IsValid)
			{
				var model = Service.Create(HttpContext.Items["StorageId"]);
				var result = await model.AccountManager.Update(account.TheAccount).ToTask();
				model.Release();
				return View(account);
			}

			return RedirectToAction("Index");
		}
	}
}