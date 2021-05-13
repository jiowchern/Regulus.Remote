using System;
using System.Net.Sockets;

namespace Regulus.Network.Rudp
{
    internal class Peer : IPeer
    {
        private readonly Regulus.Network.Socket _RudpSocket;

        public Peer(Regulus.Network.Socket rudp_socket)
        {
            _RudpSocket = rudp_socket;

        }

        event Action<SocketError> IPeer.SocketErrorEvent
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }

        IWaitableValue<int> IStreamable.Receive(byte[] buffer, int offset, int count)
        {
            return _RudpSocket.Receive(buffer, offset, count);

        }
        IWaitableValue<int> IStreamable.Send(byte[] buffer, int offset, int length)
        {
            return _RudpSocket.Send(buffer, offset, length);
        }


    }
}