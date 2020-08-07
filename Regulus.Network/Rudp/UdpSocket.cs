using Regulus.Network.Package;
using System.Net;
using System.Net.Sockets;

namespace Regulus.Network.Rudp
{
    public class UdpSocket : ISocket
    {
        private readonly System.Net.Sockets.Socket m_Socket;
        private readonly SocketSender m_Sender;
        private readonly SocketRecevier m_Receiver;


        public UdpSocket()
        {
            System.Net.Sockets.Socket socket = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            m_Socket = socket;

            m_Sender = new SocketSender(m_Socket);
            m_Receiver = new SocketRecevier(m_Socket, SocketMessageFactory.Instance);

        }
        SocketMessage[] ISocketRecevieable.Received()
        {
            return m_Receiver.Received();
        }

        void ISocketSendable.Transport(SocketMessage Message)
        {

            m_Sender.Transport(Message);
        }




        void ISocket.Close()
        {
            m_Sender.Stop();
            m_Socket.Close();
        }

        void ISocket.Bind(int Port)
        {
            m_Socket.Bind(new IPEndPoint(IPAddress.Any, Port));
            m_Receiver.Start();
        }
    }
}