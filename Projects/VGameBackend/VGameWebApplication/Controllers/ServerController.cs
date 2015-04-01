using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VGameWebApplication.Controllers
{
    [Authorize]
    public class ServerController : AsyncController
    {
        private FishHunterFormulaServiceReference.FormulaServiceClient _FormulaServiceClient;
        public ServerController()
        {
            _FormulaServiceClient = new FishHunterFormulaServiceReference.FormulaServiceClient("BasicHttpBinding_IFormulaService", "http://127.0.0.1:38972/Formula");
            
        }
        //
        // GET: /Server/
        public ActionResult Dashboard(string server)
        {
            return View();
        }

        
        public void ServerStateAsync()
        {
            //return PartialView(_FormulaServiceClient);
            
            AsyncManager.OutstandingOperations.Increment();
            System.Timers.Timer timer = new System.Timers.Timer(1.0f);
            timer.Elapsed += (sender, e) =>
            {

                AsyncManager.OutstandingOperations.Decrement();
            };
            timer.Start();
        }
        
        public ActionResult ServerStateCompleted()
        {
            return PartialView(_FormulaServiceClient);
        }
        
	}
}