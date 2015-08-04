// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectInstaller.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the ProjectInstaller type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System.ComponentModel;
using System.Configuration.Install;

#endregion

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