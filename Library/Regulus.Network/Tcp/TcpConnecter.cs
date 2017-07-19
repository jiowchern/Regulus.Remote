using System;
using System.Net;
using System.Net.Sockets;
using Regulus.Utility;

namespace Regulus.Network
{
    public class TcpConnecter : TcpSocket , ISocketConnectable
    {
        private Action<bool> _ResultHandler;
        public TcpConnecter() : base(new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
        {            
            _Socket.NoDelay = true;
        }

        void ISocketConnectable.Connect(EndPoint endpoint, Action<bool> result)
        {
            _ResultHandler = result;
            _Socket.BeginConnect(endpoint, _Result, null);
        }

        private void _Result(IAsyncResult ar)
        {
            var result = false;
            try
            {
                _Socket.EndConnect(ar);
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
                _ResultHandler(result);
                Singleton<Log>.Instance.WriteInfo(string.Format("connect result {0}.", result));
            }
        }
    }
}