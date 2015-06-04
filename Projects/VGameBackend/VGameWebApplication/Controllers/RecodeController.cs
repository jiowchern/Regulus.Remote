using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;

using Regulus.Extension;

namespace VGameWebApplication.Controllers
{
    public class RecodeController : Controller
    {
        // GET: Recode
        public ActionResult Index()
        {
            //var service = VGame.Project.FishHunter.Storage.Service.Create(HttpContext.Items["StorageId"]);

            //service.AccountManager

            var service = VGame.Project.FishHunter.Storage.Service.Create(HttpContext.Items["StorageId"]);

            var accounts = service.AccountManager.QueryAllAccount().WaitResult();

            //var provider = _User.QueryProvider<IRecordQueriers>();

            //int money = provider.Ghosts[0].Load(accounts[0].Id).WaitResult().Money;


            //var result = await model.AccountFinder.FindAccountById(accountId).ToTask();
            

            return View(accounts);
        }

        public ActionResult AddMoney()
        {
            return View();
        }

    }
}