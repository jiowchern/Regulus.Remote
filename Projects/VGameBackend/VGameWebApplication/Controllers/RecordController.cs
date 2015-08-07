// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RecordController.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the RecordController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System.Web.Mvc;

using Regulus.Net45;

using VGame.Project.FishHunter.Common;
using VGame.Project.FishHunter.Common.Data;


using VGameWebApplication.Models;
using VGameWebApplication.Storage;

#endregion

namespace VGameWebApplication.Controllers
{
	public class RecordController : Controller
	{
		public ActionResult Index()
		{
			var service = Service.Create(HttpContext.Items["StorageId"]);

			var tradeData = new TradeData();

			var accs = service.AccountManager.QueryAllAccount().WaitResult();

			foreach (var a in accs)
			{
				var money = service.RecordHandler.Load(a.Id).WaitResult().Money;

				tradeData.Add(a.Id, a.Name, money);
			}

			return View(tradeData.Datas.ToArray());
		}

		public ActionResult InputMoney(TradeData.Data data)
		{
			if (data.Deposit == 0)
			{
				return View(data);
			}

			var service = Service.Create(HttpContext.Items["StorageId"]);

			var t = new TradeNotes.TradeData(service.ConnecterId, data.OwnerId, data.Deposit, 0);
			service.TradeNotes.Write(t).WaitResult();

			return RedirectToAction("Index");
		}
	}
}