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
        
        public ServerController()
        {
            
            
        }
        //
        // GET: /Server/
        public ActionResult Dashboard(string server)
        {
            return View();
        }

        
        
        
        public ActionResult ServerState()
        {
            return PartialView(null);
        }
        
	}
}