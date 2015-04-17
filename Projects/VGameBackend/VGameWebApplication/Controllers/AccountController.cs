using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace VGameWebApplication.Controllers
{

    [Authorize]
    public class AccountController : AsyncController
    {
        private VGame.Project.FishHunter.Data.Account[] _Accounts;


        
        //
        // GET: /Account/
        public void IndexAsync()
        {
            AsyncManager.OutstandingOperations.Increment();
            var id = (Guid)HttpContext.Items["StorageId"];
            var model = new VGameWebApplication.Models.StorageApi(id);
            model.AccountManager.QueryAllAccount().OnValue += _GetAccounts;            
        }

        public ActionResult IndexCompleted()
        {
            return View(_Accounts);
        }

        private void _GetAccounts(VGame.Project.FishHunter.Data.Account[] obj)
        {
            _Accounts = obj;
            AsyncManager.OutstandingOperations.Decrement();
        }

        public ActionResult Add()
        {
            return View();
        }

        public async System.Threading.Tasks.Task<ActionResult> Edit(string account)
        {
            Guid accountId;
            if(Guid.TryParse(account , out accountId) == false)
                return RedirectToAction("Index");

            AsyncManager.OutstandingOperations.Increment();
            var id = (Guid)HttpContext.Items["StorageId"];
            var model = new VGameWebApplication.Models.StorageApi(id);

            var handler = new VGameWebApplication.Storage.Handler.FindAccount(model.AccountFinder, accountId);
            var result = await handler.Handle();
            if (result.Id != Guid.Empty)
            {
                return View(result);
            }

            return RedirectToAction("Index");
        }

        
        public ActionResult EditCompleted()
        {            
            return View();
        }

        public ActionResult Modify(VGame.Project.FishHunter.Data.Account account)
        {
            return RedirectToAction("Index");
        }
        
	}
}