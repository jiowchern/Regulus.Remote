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
        // GET: Recode
        public ActionResult Index()
        {
            VGame.Project.FishHunter.Storage.Service service = VGame.Project.FishHunter.Storage.Service.Create(HttpContext.Items["StorageId"]);
            VGameWebApplication.Models.RecordData record = new Models.RecordData();

            record.RecordManager = service.TradeAccount != null;
            
            var accs = service.AccountManager.QueryAllAccount().WaitResult();
            var money = service.TradeAccount.Find(accs[0].Id).WaitResult();

            //return PartialView(record);            
            return View(accs);
        }

        public ActionResult AddMoney()
        {
            VGame.Project.FishHunter.Storage.Service service = VGame.Project.FishHunter.Storage.Service.Create(HttpContext.Items["StorageId"]);
            VGameWebApplication.Models.RecordData record = new Models.RecordData();
            
            record.RecordFinder = service.TradeAccount != null;

            var accs = service.AccountManager.QueryAllAccount().WaitResult();
            var money = service.TradeAccount.Find(accs[0].Id).WaitResult();

            return View(record);
        }

    }
}