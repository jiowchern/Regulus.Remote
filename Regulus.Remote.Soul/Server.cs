using System;
using System.Threading;


using Regulus.Utility;
using Regulus.Utility;

namespace Regulus.Remote.Soul
{
	/// <summary>
	///     伺服器端
	/// </summary>
	public class Service : IBootable
	{


		private readonly ThreadSocketHandler _ThreadSocketHandler;
		readonly IEntry _Entry;

	    /// <summary>
		///     網路核心每秒執行次數
		/// </summary>
		public int PeerFPS
		{
			get { return _ThreadSocketHandler.FPS; }
		}

		

		/// <summary>
		///     網路核心使用率
		/// </summary>
		public float PeerUsage
		{
			get { return _ThreadSocketHandler.Power; }
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
		public long WaitingForReadPackages
		{
			get { return Peer.TotalRequest; }
		}

		/// <summary>
		///     等待寫入的封包
		/// </summary>
		public long WaitingToWrittenPackages
		{
			get { return Peer.TotalResponse; }
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="Server" /> class.
		/// </summary>
		/// <param name="entry">
		///     進入點物件
		/// </param>		
		public Service(IEntry entry,int port , IProtocol protocol, Regulus.Network.IListenable server)
		{
			_Entry = entry;
			
			_ThreadSocketHandler = new ThreadSocketHandler(port,  protocol , server);

		}

		void IBootable.Launch()
		{
			_Entry.Launch();
			Launch();
		}

		void IBootable.Shutdown()
		{
			Shutdown();
			_Entry.Shutdown();
		}

		/// <summary>
		///     啟動系統
		/// </summary>
		public void Launch()
		{
			_ThreadSocketHandler.BinderEvent += _Bind;
			_ThreadSocketHandler.Start();
            

        }

	   

	    /// <summary>
		///     關閉系統
		/// </summary>
		public void Shutdown()
		{
			
			_ThreadSocketHandler.Stop();
			_ThreadSocketHandler.BinderEvent -= _Bind;

		}

        private void _Bind(IBinder binder)
        {
			_Entry.AssignBinder(binder);
        }
    }
}
