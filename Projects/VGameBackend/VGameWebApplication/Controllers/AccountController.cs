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


        public ActionResult ModifyPassword(string password1)
        {
            var id = (Guid)HttpContext.Items["StorageId"];
            var model = new VGameWebApplication.Models.StorageApi(id);
            model.AccountManager.UpdatePassword(model.Account, password1);
            return RedirectToAction("Index");
        }
        public ActionResult Password(string account)
        {
            Guid id;
            if(Guid.TryParse(account , out id))
                return View(id);
            return RedirectToAction("Index");
            
        }
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
	}
}