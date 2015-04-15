using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace VGameWebApplication.Controllers
{
    public class HomeController : AsyncController
    {
      
        public ActionResult Logout()
        {

            FormsAuthentication.SignOut();
            HttpCookie authCookie = Request.Cookies[System.Web.Security.FormsAuthentication.FormsCookieName];

            FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
            Guid id;
            if(Guid.TryParse(authTicket.UserData , out id) )
            {
                VGame.Project.FishHunter.Storage.Appliction.Instance.Destroy(id);
            }
            
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
        public async System.Threading.Tasks.Task<ActionResult> Verify(string user, string password)
        {
            var id = await VGame.Project.FishHunter.Storage.Appliction.Instance.Create("127.0.0.1", 38973, user, password);
            if (id != Guid.Empty)
            {
                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket
                    (1
                    ,user,
                    DateTime.Now,
                    DateTime.Now.AddSeconds(300),
                    false,
                    id.ToString());

                string encTicket = FormsAuthentication.Encrypt(ticket);
                var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
                Response.Cookies.Add(cookie);
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Verify");
        }

	}




}