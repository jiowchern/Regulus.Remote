using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FormulaWebApplication.Models
{
    public class FormulaServerStatu
    {
        public int CoreFPS { get; set; }
    }
    public class FormulaServerStatuDataContext
    {
        public static FormulaServerStatu Get()
        {

            

            FormulaServiceReference.FormulaServiceClient client = new FormulaServiceReference.FormulaServiceClient("BasicHttpBinding_IFormulaService");
            return new FormulaServerStatu { CoreFPS = client.GetCoreFPS() };
        }
    }
}