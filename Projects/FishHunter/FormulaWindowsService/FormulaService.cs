using System.ServiceModel;
using System.ServiceProcess;


using VGame.Project.FishHunter.WCF;

namespace VGame.Project.FishHunter
{
	partial class FormulaService : ServiceBase
	{
		private readonly WCF.FormulaService _FormulaService;

		private readonly ServiceHost _GameHost;

		public FormulaService()
		{
			InitializeComponent();
			_FormulaService = new WCF.FormulaService();
			_GameHost = new ServiceHost(_FormulaService);
		}

		protected override void OnStart(string[] args)
		{
			_FormulaService.Launch();

			_GameHost.AddServiceEndpoint(typeof(IFormulaService), new BasicHttpBinding(), "http://localhost:38972/Formula");
			_GameHost.Open();
		}

		protected override void OnStop()
		{
			_GameHost.Close();
			_FormulaService.Shutdown();
		}
	}
}
