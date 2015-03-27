using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regulus.Remoting.Soul.Native
{
    public class Server : Regulus.Framework.ILaunched
    {
        ThreadSocketHandler _ThreadSocketHandler;
        ThreadCoreHandler _ThreadCoreHandler;


        public int PeerFPS { get { return _ThreadSocketHandler.FPS; } }
        public int CoreFPS { get { return _ThreadCoreHandler.FPS; } }
        public int PeerCount { get { return _ThreadSocketHandler.PeerCount; } }
        public Server(Regulus.Utility.ICore core,int port)
        {
            _ThreadCoreHandler = new ThreadCoreHandler(core);
            _ThreadSocketHandler = new ThreadSocketHandler(port, _ThreadCoreHandler);
        }
        void Framework.ILaunched.Launch()
        {
            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(_ThreadCoreHandler.DoWork));
            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(_ThreadSocketHandler.DoWork));
        }

        void Framework.ILaunched.Shutdown()
        {
            _ThreadCoreHandler.Stop();
            _ThreadSocketHandler.Stop();        
        }
    }
}
