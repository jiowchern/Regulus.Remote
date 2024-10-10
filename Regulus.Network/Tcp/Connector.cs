using Regulus.Utility;
using System;
using System.Net.Sockets;

namespace Regulus.Network.Tcp
{
    public class Connector
    {
        private readonly Socket _Socket;
        

        public Connector()
        {
            _Socket = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        
        }
        public System.Threading.Tasks.Task<Peer> Connect(System.Net.EndPoint endpoint)
        {        
            return System.Threading.Tasks.Task<Peer>.Factory.FromAsync(
                               (handler, obj) => _Socket.BeginConnect(endpoint, handler, null), _ResultConnect, null);
        }

        private Peer _ResultConnect(IAsyncResult ar)
        {
            _Socket.EndConnect(ar);
            return new Peer(_Socket);
        }

        public System.Threading.Tasks.Task Disconnect(bool reuse = false)
        {
            if (_Socket.Connected)
            {
                _Socket.Shutdown(SocketShutdown.Both);
                return System.Threading.Tasks.Task.Factory.FromAsync(
                            (handler, obj) => _Socket.BeginDisconnect(reuse, handler, null),
                            _Socket.EndDisconnect,
                            null);
                
            }
            else
            {
                return System.Threading.Tasks.Task.CompletedTask;
            }
        }

       
    }
}