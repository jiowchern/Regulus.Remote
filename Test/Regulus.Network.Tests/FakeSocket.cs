using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regulus.Framework;
using Regulus.Network.RUDP;
using Regulus.Utility;

namespace Regulus.Network.Tests
{
    public class FakeSocket : IRecevieable, ISendable, IUpdatable<Timestamp>
    {
        public readonly IPEndPoint Endpoint;
        private List<SocketPackage> _Packages;
        public FakeSocket(IPEndPoint endpoint)
        {
            _Packages = new List<SocketPackage>();
            Endpoint = endpoint;
        }

        

        void IBootable.Launch()
        {
            
        }

        void IBootable.Shutdown()
        {
            
        }

        public void Receive(SocketPackage package)
        {
            Assert.AreNotEqual(Endpoint, package.EndPoint);
            _Packages.Add(package);

        }
        public event Action<SocketPackage> SendEvent;
        void ISendable.Transport(SocketPackage package)
        {
            Assert.AreNotEqual(Endpoint , package.EndPoint);            
            SendEvent(package);
        }

        bool IUpdatable<Timestamp>.Update(Timestamp arg)
        {
            return true;
        }

        public SocketPackage[] Received()
        {
            var pkgs = _Packages.ToArray();
            _Packages.Clear();
            return pkgs;
        }

        EndPoint[] IRecevieable.ErrorPoints()
        {
            return new EndPoint[0];
        }
    }
}