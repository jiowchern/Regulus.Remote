using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;

using Regulus.Extension;


namespace VGameWebApplication.Controllers
{
    public class RecordController : Controller
    {
        public ActionResult Index()
        {
            VGame.Project.FishHunter.Storage.Service service = VGame.Project.FishHunter.Storage.Service.Create(HttpContext.Items["StorageId"]);

            Models.TradeData tradeData = new Models.TradeData();

            
            var accs = service.AccountManager.QueryAllAccount().WaitResult();

            foreach(var a in accs)
            {
                var money = service.RecodeQueriers.Load(a.Id).WaitResult().Money;
                
                tradeData.Add(a.Id, a.Name, money);
            }

            return View(tradeData.Datas.ToArray()); 
        }

        public ActionResult InputMoney(Models.TradeData.Data data)
        {
            if(data.Deposit == 0)
            {
                return View(data);
            }

            VGame.Project.FishHunter.Storage.Service service = VGame.Project.FishHunter.Storage.Service.Create(HttpContext.Items["StorageId"]);

            var t = new VGame.Project.FishHunter.Data.TradeNotes.TradeData(service.ConnecterId, data.OwnerId, data.Deposit, 0);
            service.TradeNotes.Write(t).WaitResult();

            return RedirectToAction("Index");
        }

    }
}