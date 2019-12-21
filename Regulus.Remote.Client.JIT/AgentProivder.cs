using System;

namespace Regulus.Remote.Client.JIT
{
    public static class AgentProivder
    {
        
        public static Regulus.Remote.IAgent CreateRudp(Protocol.Essential essential )
        {
            var client = new Regulus.Network.Rudp.ConnectProvider(new Regulus.Network.Rudp.UdpSocket());

            
            var ab = new Regulus.Remote.Protocol.AssemblyBuilder(essential);
            var protocolAsm = ab.Create();
            var protocol = Regulus.Remote.Protocol.ProtocolProvider.Create(protocolAsm);
            var agent = new Regulus.Remote.Ghost.Agent(protocol , client);
            return agent;
        }

        public static Regulus.Remote.IAgent CreateTcp(Protocol.Essential essential)
        {
            
            var client = new  Regulus.Network.Tcp.ConnectProvider();
            
            var ab = new Regulus.Remote.Protocol.AssemblyBuilder(essential);
            var protocolAsm = ab.Create();
            var protocol = Regulus.Remote.Protocol.ProtocolProvider.Create(protocolAsm);
            var agent = new Regulus.Remote.Ghost.Agent(protocol, client);
            return agent;
        }

    }
}

