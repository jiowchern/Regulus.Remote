using Regulus.Remote.Ghost;
using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Regulus.Remote.Client
{
    public class TcpSocket
    {
        private readonly IAgent _Agent;
        readonly Regulus.Network.IConnectable _Connectable;
        readonly Regulus.Network.IPeer _Peer;
        readonly Regulus.Network.Tcp.Connecter _Connecter;

        public event System.Action<bool> ConnectedAsyncEvent;
        public event System.Action<System.Net.Sockets.SocketError> DisconnectAsyncEvent;
        public TcpSocket(IAgent agent)
        {
            ConnectedAsyncEvent += _Empty;            
            this._Agent = agent;
            _Connecter = new Network.Tcp.Connecter();            
            
            _Connectable = _Connecter;
            _Peer = _Connecter;
            _Peer.SocketErrorEvent += _Disconnect;
            _Peer.SocketErrorEvent += DisconnectAsyncEvent;
        }

        private void _Disconnect(SocketError obj)
        {
            _Agent.Stop();
        }

        private void _Empty(bool result)
        {
            
        }

        public async Task Connect(System.Net.EndPoint endpoint)
        {
            var result = _Connectable.Connect(endpoint);
            await result.ContinueWith(_ConnectResult);
        }
        public async Task Disconnect()
        {
            await _Connectable.Disconnect();
        }
        

        private void _ConnectResult(System.Threading.Tasks.Task<bool> task)
        {
            if(task.Result)
            {
                _Agent.Start(_Connecter);                
            }
            ConnectedAsyncEvent(task.Result);
        }
    }
}