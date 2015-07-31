// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Service1.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the Service1 type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System.ServiceProcess;

#endregion

namespace WindowsService
{
	public partial class Service1 : ServiceBase
	{
		public Service1()
		{
			InitializeComponent();
		}

		protected override void OnStart(string[] args)
		{
		}

		protected override void OnStop()
		{
		}
	}
}