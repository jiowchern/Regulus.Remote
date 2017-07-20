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
        private readonly List<SocketMessage> _Packages;
        public FakeSocket(IPEndPoint endpoint)
        {
            _Packages = new List<SocketMessage>();
            Endpoint = endpoint;
        }

        

        void IBootable.Launch()
        {
            
        }

        void IBootable.Shutdown()
        {
            
        }

        public void Receive(SocketMessage message)
        {
            Assert.AreNotEqual(Endpoint, message.EndPoint);
            _Packages.Add(message);

        }
        public event Action<SocketMessage> SendEvent;
        void ISendable.Transport(SocketMessage message)
        {
            Assert.AreNotEqual(Endpoint , message.EndPoint);            
            SendEvent(message);
        }

        bool IUpdatable<Timestamp>.Update(Timestamp arg)
        {
            return true;
        }

        public SocketMessage[] Received()
        {
            var pkgs = _Packages.ToArray();
            _Packages.Clear();
            return pkgs;
        }

        
    }
}