using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Regulus.Extension;
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
            return View(new VGame.Project.FishHunter.Data.Account());
        }

        async public System.Threading.Tasks.Task<ActionResult> AddHandle(VGame.Project.FishHunter.Data.Account account)
        {
            var id = (Guid)HttpContext.Items["StorageId"];
            var model = new VGameWebApplication.Models.StorageApi(id);

            account.Id = Guid.NewGuid();
            var result = await model.AccountManager.Create(account).ToTask();
            return RedirectToAction("Index");
        }

        public async System.Threading.Tasks.Task<ActionResult> Edit(string accountid)
        {
            Guid accountId;
            if (Guid.TryParse(accountid, out accountId) == false)
                return RedirectToAction("Index");

            
            var id = (Guid)HttpContext.Items["StorageId"];
            var model = new VGameWebApplication.Models.StorageApi(id);
            var result = await model.AccountFinder.FindAccountById(accountId).ToTask();            
            if (result.Id != Guid.Empty)
            {
                var updateAccount = new VGameWebApplication.Models.UpdateAccount();
                updateAccount.TheAccount = result;
                return View(updateAccount);
            }

            return RedirectToAction("Index");
        }



        [AcceptVerbs(HttpVerbs.Post)]
        async public System.Threading.Tasks.Task<ActionResult> Edit( VGameWebApplication.Models.UpdateAccount account)
        {
            if(ModelState.IsValid)
            {
                var id = (Guid)HttpContext.Items["StorageId"];
                var model = new VGameWebApplication.Models.StorageApi(id);
                var result = await model.AccountManager.Update(account.TheAccount).ToTask();
                return View(account);
            }
            return RedirectToAction("Index");
        }
        
	}
}