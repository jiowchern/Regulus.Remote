using System;
using System.Net;
using Regulus.Network.RUDP;
using Regulus.Utility;

namespace Regulus.Network.Tests.AgentApp
{
    internal class InitialStage : IStage
    {
        private readonly Command _Command;
        private Agent _Agent;
        public event Action<IPeer> CreatedEvent;
        public InitialStage(Agent agent , Command command)
        {
            _Agent = agent;
            _Command = command;
        }

        void IStage.Enter()
        {
            _Command.RegisterLambda<InitialStage, string,int>(this, (obj, ip,port) => obj.Connect(ip,port));
        }

        private void Connect(string ip,int port)
        {
            var endPoint = new IPEndPoint(IPAddress.Parse(ip), port);            
            var peer = _Agent.Connect(endPoint);
            CreatedEvent(peer);
        }

        void IStage.Leave()
        {
            _Command.Unregister("Connect");
        }

        void IStage.Update()
        {            
        }
    }
}