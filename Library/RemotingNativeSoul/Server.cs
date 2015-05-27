using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regulus.Remoting.Soul.Native
{

    /// <summary>
    /// 伺服器端
    /// </summary>
    public class Server : Regulus.Framework.IBootable
    {
        ThreadSocketHandler _ThreadSocketHandler;
        ThreadCoreHandler _ThreadCoreHandler;

        /// <summary>
        /// 網路核心每秒執行次數
        /// </summary>
        public int PeerFPS { get { return _ThreadSocketHandler.FPS; } }

        /// <summary>
        /// 邏輯核心每秒執行次數
        /// </summary>
        public int CoreFPS { get { return _ThreadCoreHandler.FPS; } }

        /// <summary>
        /// 網路核心使用率
        /// </summary>
        public float PeerUsage { get { return _ThreadSocketHandler.Power; } }

        /// <summary>
        /// 邏輯核心使用率
        /// </summary>
        public float CoreUsage { get { return _ThreadCoreHandler.Power; } }

        /// <summary>
        /// 客戶端連線數
        /// </summary>
        public int PeerCount { get { return _ThreadSocketHandler.PeerCount; } }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="core">進入點物件</param>
        /// <param name="port">監聽的埠</param>
        public Server(Regulus.Remoting.ICore core,int port)
        {
            _ThreadCoreHandler = new ThreadCoreHandler(core);
            _ThreadSocketHandler = new ThreadSocketHandler(port, _ThreadCoreHandler);
        }

        /// <summary>
        /// 啟動系統
        /// </summary>
        public void Launch()
        {
            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(_ThreadCoreHandler.DoWork));
            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(_ThreadSocketHandler.DoWork));
        }
        void Framework.IBootable.Launch()
        {
            Launch();
        }


        /// <summary>
        /// 關閉系統
        /// </summary>
        public void Shutdown()
        {
            _ThreadCoreHandler.Stop();
            _ThreadSocketHandler.Stop();        
        }

        
        void Framework.IBootable.Shutdown()
        {
            Shutdown();
        }


        /// <summary>
        /// 總共讀取位元
        /// </summary>
        public long TotalReadBytes { get { return (int)NetworkMonitor.Instance.Read.TotalBytes; } }

        /// <summary>
        /// 總共寫入位元
        /// </summary>
        public long TotalWriteBytes {  get { return (int)NetworkMonitor.Instance.Write.TotalBytes; } }

        /// <summary>
        /// 每秒讀取位元
        /// </summary>
        public int ReadBytesPerSecond { get { return (int)NetworkMonitor.Instance.Read.SecondBytes; } }


        /// <summary>
        /// 每秒寫入位元
        /// </summary>
        public int WriteBytesPerSecond { get { return (int)NetworkMonitor.Instance.Write.SecondBytes; } }


        /// <summary>
        /// 等待讀取的封包
        /// </summary>
        public int WaitingForReadPackages { get { return Peer.TotalRequest; } }


        /// <summary>
        /// 等待寫入的封包
        /// </summary>
        public int WaitingToWrittenPackages { get { return Peer.TotalResponse; } }
    }
}
