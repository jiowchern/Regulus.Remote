using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace Regulus.Network.RUDP
{
    public class SocketRecevier : IRecevieable
    {
        private readonly Socket _Socket;
        private readonly byte[] _Buffer;
        private EndPoint _EndPoint;
        

        public SocketRecevier(Socket socket , int package_size)
        {
            _Socket = socket;
            _Buffer = new byte[package_size];
            _EndPoint = new IPEndPoint(IPAddress.Any, 0);
            _Begin();
        }

        private void _Begin()
        {
            _Socket.BeginReceiveFrom(_Buffer, 0, _Buffer.Length , SocketFlags.None, ref _EndPoint , _End , null);
        }

        private void _End(IAsyncResult ar)
        {
            EndPoint sourcEndPoint = new IPEndPoint( IPAddress.Any, 0);
            _Socket.EndReceiveFrom(ar, ref sourcEndPoint);

            var package = new SocketPackage();
            package.EndPoint = sourcEndPoint;
            package.Buffer = _Buffer.ToArray();
            _ReceivedEvent(package);

            _Begin();
        }

        private event Action<SocketPackage> _ReceivedEvent;

        event Action<SocketPackage> IRecevieable.ReceivedEvent
        {
            add { this._ReceivedEvent += value; }
            remove { this._ReceivedEvent -= value; }
        }
    }
}