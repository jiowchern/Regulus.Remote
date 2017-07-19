using System;
using Regulus.Network.RUDP;
using Regulus.Utility;

namespace Regulus.Network.Tests.AgentApp
{
    internal class ConnectStage : IStage
    {
        private readonly Command _Command;
        private readonly ISocket _Peer;

        public event Action SuccessEvent;

        

        public ConnectStage(ISocket peer, Command command)
        {
            _Peer = peer;
            _Command = command;        
        }

        void IStage.Enter()
        {
            
        }

        void IStage.Leave()
        {            
        }

        void IStage.Update()
        {
            
            if (_Peer.Connected)
                SuccessEvent();
        }
    }
}