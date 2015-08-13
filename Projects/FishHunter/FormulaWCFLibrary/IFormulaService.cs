using System;
using System.ServiceModel;

namespace VGame.Project.FishHunter.WCF
{
	// 注意: 您可以使用 [重構] 功能表上的 [重新命名] 命令同時變更程式碼和組態檔中的介面名稱 "IFormulaService"。
	[Serializable]
	public struct FormulaState
	{
		public int CoreFps;

		public int PeerCount;

		public int PeerFps;

		public int ReadBytesPerSecond;

		public long TotalReadBytes;

		public long TotalWriteBytes;

		public int WattToRead;

		public int WattToWrite;

		public int WriteBytesPerSecond;
	}

	[ServiceContract]
	public interface IFormulaService
	{
		[OperationContract]
		FormulaState GetState();
	}
}
