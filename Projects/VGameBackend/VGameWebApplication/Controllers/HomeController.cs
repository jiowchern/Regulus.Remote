using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace VGameWebApplication.Controllers
{
    public class HomeController : Controller
    {
      
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Verify");
        }
        //
        // GET: /Home/        
        public ActionResult Index()
        {

            if (User.Identity.IsAuthenticated == false)            
            {                
                return RedirectToAction("Verify");
            }
            return View();
        }

        //
        // GET: /Home/        
        public ActionResult Verify()
        {            
            return View();
        }


        
        
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Verify(string user ,string password)
        {
                        

            FormsAuthentication.RedirectFromLoginPage(user, false);            

            return RedirectToAction("Index", "Home");
        }
	}




}