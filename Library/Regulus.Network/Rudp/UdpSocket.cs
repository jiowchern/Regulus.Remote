using System;
using System.Net;
using System.Net.Sockets;

namespace Regulus.Network.RUDP
{
    public class UdpSocket : ISocket
    {
        private readonly Socket _Socket;
        private readonly SocketSender _Sender;
        private readonly SocketRecevier _Receiver;
        

        public UdpSocket()
        {
            var socket = new System.Net.Sockets.Socket(System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Dgram, System.Net.Sockets.ProtocolType.Udp);

            _Socket = socket;
            _Sender = new SocketSender(_Socket);
            _Receiver = new SocketRecevier(_Socket);
            
        }
        SocketMessage[] ISocketRecevieable.Received()
        {
            return _Receiver.Received();
        }

        void ISocketSendable.Transport(SocketMessage message)
        {
            _Sender.Transport(message);
        }

        event Action<SocketMessage> ISocketSendable.DoneEvent
        {
            add { _Sender.DoneEvent += value; }
            remove { _Sender.DoneEvent -= value; }
        }

        void ISocket.Close()
        {
            _Socket.Close();
        }

        void ISocket.Bind(int port)
        {
            _Socket.Bind(new IPEndPoint(IPAddress.Any, port));
            _Receiver.Start();
        }
    }
}