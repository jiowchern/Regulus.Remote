using Regulus.Network.Tcp;
using Regulus.Remote.Ghost;
using System;
using System.Net.Sockets;

namespace Regulus.Remote.Client.Tcp
{
    class Online : IOnlineable
    {
        
        readonly Network.IConnectable connectable;
        readonly Peer peer;
        private IAgent agent;

        public Online(Network.Tcp.Connecter connecter, IAgent agent)
        {
            _ErrorEvent += (s) => { };
        
            connectable = connecter;
            peer = connecter;
            this.agent = agent;

            peer.SocketErrorEvent += _Error;
            agent.Start(peer);
        }

        private void _Error(SocketError obj)
        {
            _ErrorEvent(obj);
            _Disconnect();
        }

        event Action<SocketError> _ErrorEvent;
        event Action<SocketError> IOnlineable.ErrorEvent
        {
            add
            {
                _ErrorEvent += value;
            }

            remove
            {
                _ErrorEvent -= value;
            }
        }

        void IOnlineable.Disconnect()
        {
            _Disconnect();
        }

        private void _Disconnect()
        {
            agent.Stop();
            connectable.Disconnect();
            peer.SocketErrorEvent -= _Error;

        }
    }
}