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
        private readonly byte[] _Buffer;
        private EndPoint _EndPoint;
        private readonly List<SocketPackage> _ReceivePackages;

        private readonly HashSet<EndPoint> _Errors;
        private readonly SocketPackage[] _Empty;

        public SocketRecevier(Socket socket , int package_size)
        {
            _Empty = new SocketPackage[0];
            _Socket = socket;
            _Buffer = new byte[package_size];
            _EndPoint = new IPEndPoint(IPAddress.Any, 0);
            _ReceivePackages = new List<SocketPackage>();
            _Errors = new HashSet<EndPoint>();
            _Begin();
        }

        private void _Begin()
        {
            try
            {
                _Socket.BeginReceiveFrom(_Buffer, 0, _Buffer.Length, SocketFlags.None, ref _EndPoint, _End, null);
            }
            catch (Exception e)
            {
                lock (_Errors)
                {
                    _Errors.Add(_EndPoint);
                }
            }
            
        }

        private void _End(IAsyncResult ar)
        {
            EndPoint sourcEndPoint = new IPEndPoint( IPAddress.Any, 0);
            try
            {
                _Socket.EndReceiveFrom(ar, ref sourcEndPoint);
            }
            catch (Exception e)
            {
                lock (_Errors)
                {
                    _Errors.Add(sourcEndPoint);
                }
            }
            

            var package = new SocketPackage();
            package.EndPoint = sourcEndPoint;
            package.Buffer = _Buffer.ToArray();
            lock (_ReceivePackages)
            {
                _ReceivePackages.Add(package);
            }

            _Begin();
        }


        SocketPackage[] IRecevieable.Received()
        {
            var pkgs = _Empty;
            lock (_ReceivePackages)
            {
                pkgs = _ReceivePackages.ToArray();
                _ReceivePackages.Clear();
            }

            return pkgs;
        }

        EndPoint[] IRecevieable.ErrorPoints()
        {
            lock (_Socket)
            {
                var errors = _Errors.ToArray();
                _Errors.Clear();
                return errors;
            }
        }
    }
}