using System.Threading;


using Regulus.Framework;
using Regulus.Utility;

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
			get { return _ThreadSocketHandler.FPS; }
		}

		/// <summary>
		///     邏輯核心每秒執行次數
		/// </summary>
		public int CoreFPS
		{
			get { return _ThreadCoreHandler.FPS; }
		}

		/// <summary>
		///     網路核心使用率
		/// </summary>
		public float PeerUsage
		{
			get { return _ThreadSocketHandler.Power; }
		}

		/// <summary>
		///     邏輯核心使用率
		/// </summary>
		public float CoreUsage
		{
			get { return _ThreadCoreHandler.Power; }
		}

		/// <summary>
		///     客戶端連線數
		/// </summary>
		public int PeerCount
		{
			get { return _ThreadSocketHandler.PeerCount; }
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
		///     Initializes a new instance of the <see cref="Server" /> class.
		/// </summary>
		/// <param name="core">
		///     進入點物件
		/// </param>
		/// <param name="port">
		///     監聽的埠
		/// </param>
		public Server(ICore core, int port)
		{
			_ThreadCoreHandler = new ThreadCoreHandler(core);
			_ThreadSocketHandler = new ThreadSocketHandler(port, _ThreadCoreHandler);

			_WaitSocket = new AutoResetEvent(false);
		}

		void IBootable.Launch()
		{
			Launch();
		}

		void IBootable.Shutdown()
		{
			Shutdown();
		}

		/// <summary>
		///     啟動系統
		/// </summary>
		public void Launch()
		{
			ThreadPool.QueueUserWorkItem(_ThreadCoreHandler.DoWork);
			ThreadPool.QueueUserWorkItem(_ThreadSocketHandler.DoWork, _WaitSocket);
		}

		/// <summary>
		///     關閉系統
		/// </summary>
		public void Shutdown()
		{
			_ThreadCoreHandler.Stop();
			_ThreadSocketHandler.Stop();

			WaitHandle.WaitAll(
				new[]
				{
					_WaitSocket
				});
		}
	}
}
