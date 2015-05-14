using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace VGameWebApplication
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            var asm = System.Reflection.Assembly.Load("App_Code/AssemblyInfo.cs");
            Application.Add("Assembly", asm);

            Application.Add("BuildDateTime", new System.IO.FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).LastWriteTime.ToShortDateString());

            ModelBinders.Binders.Add(typeof(Guid) , new GuidModelBinder()); 
        }

        protected void Application_PostAuthenticateRequest(Object sender, EventArgs e)
        {
            HttpCookie authCookie = Request.Cookies[System.Web.Security.FormsAuthentication.FormsCookieName];
            if(authCookie != null)
            {
                FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                Guid id;
                if(Guid.TryParse(authTicket.UserData , out id) == false || VGame.Project.FishHunter.Storage.Appliction.Instance.Valid(id) == false)
                {
                    HttpContext.Current.User = new System.Security.Principal.GenericPrincipal(new System.Security.Principal.GenericIdentity(""), new string[0]);                    
                }
                else
                    HttpContext.Current.Items["StorageId"] = id;
            }

        }
    }
}
