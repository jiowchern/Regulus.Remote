using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace VGame.Project.FishHunter
{

    
    partial class FormulaService : ServiceBase
    {
        VGame.Project.FishHunter.WCF.FormulaService _FormulaService;
        ServiceHost _GameHost;
        public FormulaService()
        {
            InitializeComponent();
            _FormulaService = new WCF.FormulaService();
            _GameHost = new ServiceHost(_FormulaService);
        }
        
        protected override void OnStart(string[] args)
        {
            _FormulaService.Launch();

            _GameHost.AddServiceEndpoint(typeof(VGame.Project.FishHunter.WCF.IFormulaService), new BasicHttpBinding(), "http://localhost:38972/Formula");
            _GameHost.Open();
        }

        protected override void OnStop()
        {
            
            _GameHost.Close();
            _FormulaService.Shutdown();
        }

        
    }
}
