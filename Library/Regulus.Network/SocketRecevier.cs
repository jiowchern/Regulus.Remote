using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace Regulus.Network.RUDP
{
    public class SocketRecevier : IRecevieable
    {
        private readonly Socket _Socket;
        
        private readonly List<SocketMessage> _ReceivePackages;

        private readonly SocketMessage[] _Empty;

        private SocketMessage _Message;
        private readonly ISocketPackageSpawner _Spawner;
        private EndPoint _ReceiveEndPoint;
        public SocketRecevier(Socket socket )
        {
            _Empty = new SocketMessage[0];
            _Socket = socket;
            
            _ReceivePackages = new List<SocketMessage>();
            _Spawner = SocketPackagePool.Instance;

            _ReceiveEndPoint = new IPEndPoint(IPAddress.Any, 0);
            _Begin();
        }

        private void _Begin()
        {
            _Message = _Spawner.Spawn();
            
            try
            {
                _Socket.BeginReceiveFrom(_Message.Package, 0, _Message.Package.Length, SocketFlags.None, ref _ReceiveEndPoint, _End, null);
            }
            catch (SocketException e)
            {
                SocketError error = SocketError.Success;
                error = e.SocketErrorCode;
                _Message.SetError(error);
                _Message.SetEndPoint(_ReceiveEndPoint);
                lock (_ReceivePackages)
                {
                    _ReceivePackages.Add(_Message);
                }
                _Begin();
            }
        }

        private void _End(IAsyncResult ar)
        {
            SocketError error = SocketError.Success;
            try
            {
                _Socket.EndReceiveFrom(ar, ref _ReceiveEndPoint);
            }
            catch (SocketException e)
            {
                error = e.SocketErrorCode;
            }
            
            _Message.SetError(error);
            _Message.SetEndPoint(_ReceiveEndPoint);
            lock (_ReceivePackages)
            {
                _ReceivePackages.Add(_Message);
            }
            _Begin();
        }


        SocketMessage[] IRecevieable.Received()
        {
            var pkgs = _Empty;
            lock (_ReceivePackages)
            {
                pkgs = _ReceivePackages.ToArray();
                _ReceivePackages.Clear();
            }

            return pkgs;
        }

        
    }

    public class SocketPackageFactory : IObjectFactory<SocketMessageInternal>
    {
        SocketMessageInternal IObjectFactory<SocketMessageInternal>.Spawn()
        {
            return new SocketMessageInternal(Config.PackageSize);
        }
    }
}