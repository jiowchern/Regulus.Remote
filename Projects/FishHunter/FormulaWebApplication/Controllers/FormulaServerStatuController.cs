using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FormulaWebApplication.Controllers
{
    public class FormulaServerStatuController : Controller
    {
        //
        // GET: /FormulaServerStatu/
        public ActionResult Index()
        {

            return View( FormulaWebApplication.Models.FormulaServerStatuDataContext.Get() );
        }
	}
}