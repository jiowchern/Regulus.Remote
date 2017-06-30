using System;
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

        public FakeSocket(IPEndPoint endpoint)
        {
            Endpoint = endpoint;
        }

        private event Action<SocketPackage> _ReceivedEvent;

        event Action<SocketPackage> IRecevieable.ReceivedEvent
        {
            add { this._ReceivedEvent += value; }
            remove { this._ReceivedEvent -= value; }
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
            if(_ReceivedEvent != null)
                _ReceivedEvent(package);
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
    }
}