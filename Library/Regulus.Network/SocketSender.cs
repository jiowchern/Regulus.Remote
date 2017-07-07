using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Regulus.Network.RUDP
{
    class SocketSender : ISendable
    {
        private readonly Socket _Socket;
        private bool _Sending;
        private readonly Queue<SocketPackage> _Packages;
        public SocketSender(Socket socket)
        {
            _Socket = socket;
            _Packages = new Queue<SocketPackage>();
        }

        public void Transport(SocketPackage package)
        {
            if (_Sending)
            {
                _Enqueue(package);
            }
            else
            {
                _Transport(package);
            }
        }

        private void _Enqueue(SocketPackage package)
        {
            lock (_Packages)
            {
                _Packages.Enqueue(package);
            }
        }

        private void _Transport(SocketPackage package)
        {            
            if (package != null)
            {
                _Sending = true;
                _Socket.BeginSendTo(package.Buffer, 0, package.Buffer.Length, SocketFlags.None, package.EndPoint, _Done,
                    null);
            }
        }

        private void _Done(IAsyncResult ar)
        {
            _Socket.EndSendTo(ar);
            _Sending = false;
            var pkg = _Dequeue();
            if(pkg != null)
                _Transport(pkg);
        }

        private SocketPackage _Dequeue()
        {
            lock (_Packages)
            {
                if(_Packages.Count > 0)
                    return _Packages.Dequeue();
            }

            return null;
        }
    }
}