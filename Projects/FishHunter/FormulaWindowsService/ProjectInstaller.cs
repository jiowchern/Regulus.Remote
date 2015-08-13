using System.ComponentModel;
using System.Configuration.Install;

namespace VGame.Project.FishHunter
{
	[RunInstaller(true)]
	public partial class ProjectInstaller : Installer
	{
		public ProjectInstaller()
		{
			InitializeComponent();
		}

		private void serviceInstaller1_AfterInstall(object sender, InstallEventArgs e)
		{
		}
	}
}
