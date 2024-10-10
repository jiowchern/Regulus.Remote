using Regulus.Utility;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Regulus.Network.Tcp
{
   /* public class Connector : Peer 
    {
        public event System.Action ConnectedEvent;
        public event System.Action DisonnectedEvent;
        public Connector() : base(new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
        {

        }

        public System.Threading.Tasks.Task<bool> Connect(EndPoint endpoint)
        {
            System.Net.Sockets.Socket socket = GetSocket();
            return System.Threading.Tasks.Task<bool>.Factory.FromAsync(
                (handler, obj) => socket.BeginConnect(endpoint, handler, null), _ResultConnect, null);
        }

        public Task Disconnect()
        {
            System.Net.Sockets.Socket socket = GetSocket();
            return System.Threading.Tasks.Task<bool>.Factory.FromAsync(
                (handler, obj) => socket.BeginDisconnect(false, handler, null), _ResultDisconnect, null);
            
        }

        private bool _ResultDisconnect(IAsyncResult arg)
        {
            bool result = false;
            try
            {

                GetSocket().EndDisconnect(arg);
                result = true;
                if(result)
                    DisonnectedEvent?.Invoke();
            }
            catch (SystemException se)
            {
                Singleton<Log>.Instance.WriteInfo($" result disconnect {se.ToString()}.");
            }
            finally
            {

                Singleton<Log>.Instance.WriteInfo(string.Format("disconnect result {0}.", result));

            }
            return result;
        }

        private bool _ResultConnect(IAsyncResult ar)
        {
            bool result = false;
            try
            {
                
                GetSocket().EndConnect(ar);
                result = true;
                if(result)
                    ConnectedEvent?.Invoke();
            }            
            catch(SystemException se)
            {
                Singleton<Log>.Instance.WriteInfo($" result connect {se.ToString()}.");
            }
            finally
            {

                Singleton<Log>.Instance.WriteInfo(string.Format("connect result {0}.", result));

            }
            return result;
        }


    }*/
}