using System;
using System.Net;
using System.Net.Sockets;
using Regulus.Utility;

namespace Regulus.Network.Tcp
{
    public class Connecter : Peer , IConnectable
    {
        private Action<bool> m_ResultHandler;
        public Connecter() : base(new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
        {            
            GetSocket().NoDelay = true;
        }

        void IConnectable.Connect(EndPoint Endpoint, Action<bool> result)
        {
            m_ResultHandler = result;
            GetSocket().BeginConnect(Endpoint, this.Result, state: null);
        }

        private void Result(IAsyncResult Ar)
        {
            var result = false;
            try
            {
                GetSocket().EndConnect(Ar);
                result = true;
            }
            catch (SocketException ex)
            {
                Singleton<Log>.Instance.WriteInfo(ex.ToString());
            }
            catch (ObjectDisposedException ode)
            {
                Singleton<Log>.Instance.WriteInfo(ode.ToString());
            }
            finally
            {
                m_ResultHandler(result);
                m_ResultHandler = null;
                Singleton<Log>.Instance.WriteInfo(string.Format("connect result {0}.", result));
            }
        }

        
    }
}