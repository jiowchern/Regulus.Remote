// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Server.cs" company="">
//   
// </copyright>
// <summary>
//   伺服器端
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System.Threading;

using Regulus.Framework;
using Regulus.Utility;

#endregion

namespace Regulus.Remoting.Soul.Native
{
	/// <summary>
	///     伺服器端
	/// </summary>
	public class Server : IBootable
	{
		private readonly ThreadCoreHandler _ThreadCoreHandler;

		private readonly ThreadSocketHandler _ThreadSocketHandler;

		private readonly WaitHandle _WaitSocket;

		/// <summary>
		///     網路核心每秒執行次數
		/// </summary>
		public int PeerFPS
		{
			get { return this._ThreadSocketHandler.FPS; }
		}

		/// <summary>
		///     邏輯核心每秒執行次數
		/// </summary>
		public int CoreFPS
		{
			get { return this._ThreadCoreHandler.FPS; }
		}

		/// <summary>
		///     網路核心使用率
		/// </summary>
		public float PeerUsage
		{
			get { return this._ThreadSocketHandler.Power; }
		}

		/// <summary>
		///     邏輯核心使用率
		/// </summary>
		public float CoreUsage
		{
			get { return this._ThreadCoreHandler.Power; }
		}

		/// <summary>
		///     客戶端連線數
		/// </summary>
		public int PeerCount
		{
			get { return this._ThreadSocketHandler.PeerCount; }
		}

		/// <summary>
		///     總共讀取位元
		/// </summary>
		public long TotalReadBytes
		{
			get { return (int)Singleton<NetworkMonitor>.Instance.Read.TotalBytes; }
		}

		/// <summary>
		///     總共寫入位元
		/// </summary>
		public long TotalWriteBytes
		{
			get { return (int)Singleton<NetworkMonitor>.Instance.Write.TotalBytes; }
		}

		/// <summary>
		///     每秒讀取位元
		/// </summary>
		public int ReadBytesPerSecond
		{
			get { return (int)Singleton<NetworkMonitor>.Instance.Read.SecondBytes; }
		}

		/// <summary>
		///     每秒寫入位元
		/// </summary>
		public int WriteBytesPerSecond
		{
			get { return (int)Singleton<NetworkMonitor>.Instance.Write.SecondBytes; }
		}

		/// <summary>
		///     等待讀取的封包
		/// </summary>
		public int WaitingForReadPackages
		{
			get { return Peer.TotalRequest; }
		}

		/// <summary>
		///     等待寫入的封包
		/// </summary>
		public int WaitingToWrittenPackages
		{
			get { return Peer.TotalResponse; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Server"/> class. 
		/// </summary>
		/// <param name="core">
		/// 進入點物件
		/// </param>
		/// <param name="port">
		/// 監聽的埠
		/// </param>
		public Server(ICore core, int port)
		{
			this._ThreadCoreHandler = new ThreadCoreHandler(core);
			this._ThreadSocketHandler = new ThreadSocketHandler(port, this._ThreadCoreHandler);

			this._WaitSocket = new AutoResetEvent(false);
		}

		void IBootable.Launch()
		{
			this.Launch();
		}

		void IBootable.Shutdown()
		{
			this.Shutdown();
		}

		/// <summary>
		///     啟動系統
		/// </summary>
		public void Launch()
		{
			ThreadPool.QueueUserWorkItem(this._ThreadCoreHandler.DoWork);
			ThreadPool.QueueUserWorkItem(this._ThreadSocketHandler.DoWork, this._WaitSocket);
		}

		/// <summary>
		///     關閉系統
		/// </summary>
		public void Shutdown()
		{
			this._ThreadCoreHandler.Stop();
			this._ThreadSocketHandler.Stop();

			WaitHandle.WaitAll(new[]
			{
				this._WaitSocket
			});
		}
	}
}