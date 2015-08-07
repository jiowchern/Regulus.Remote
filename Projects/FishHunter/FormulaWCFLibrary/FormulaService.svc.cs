// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormulaService.svc.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the FormulaService type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System.ServiceModel;

using Regulus.Remoting.Soul.Native;
using Regulus.Utility;

using VGame.Project.FishHunter.Formula;

#endregion

namespace VGame.Project.FishHunter.WCF
{
	// 注意: 您可以使用 [重構] 功能表上的 [重新命名] 命令同時變更程式碼、svc 和組態檔中的類別名稱 "FormulaService"。
	// 注意: 若要啟動 WCF 測試用戶端以便測試此服務，請在 [方案總管] 中選取 FormulaService.svc 或 FormulaService.svc.cs，然後開始偵錯。
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
	public class FormulaService : IFormulaService
	{
		private readonly Center _Formula;

		private readonly Launcher _Launcher;

		private readonly Server _Server;

		public FormulaService()
		{
			_Formula = new Center(new ExpansionFeature());
			_Server = new Server(_Formula, 38971);
			_Launcher = new Launcher();

			_Launcher.Push(_Server);
		}

		FormulaState IFormulaService.GetState()
		{
			return new FormulaState
			{
				CoreFps = _Server.CoreFPS, 
				PeerCount = _Server.PeerCount, 
				PeerFps = _Server.PeerFPS, 
				TotalReadBytes = _Server.TotalReadBytes, 
				TotalWriteBytes = _Server.TotalWriteBytes, 
				ReadBytesPerSecond = _Server.ReadBytesPerSecond, 
				WriteBytesPerSecond = _Server.WriteBytesPerSecond, 
				WattToRead = _Server.WaitingForReadPackages, 
				WattToWrite = _Server.WaitingToWrittenPackages
			};
		}

		public void Launch()
		{
			_Launcher.Launch();
		}

		public void Shutdown()
		{
			_Launcher.Shutdown();
		}
	}
}