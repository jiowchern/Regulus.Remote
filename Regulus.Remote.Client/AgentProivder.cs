using System.Linq;

namespace Regulus.Remote.Client
{
    public static class AgentProivder
    {        

        public static Regulus.Remote.IAgent CreateRudp(System.Reflection.Assembly protocol_assembly)
        {
            
            var protocol = Regulus.Remote.Protocol.ProtocolProvider.Create(protocol_assembly);            
            return CreateRudp(protocol);
        }

        public static Regulus.Remote.IAgent CreateRudp(IProtocol protocol)
        {
            var client = new Regulus.Network.Rudp.ConnectProvider(new Regulus.Network.Rudp.UdpSocket());
            var agent = new Regulus.Remote.Ghost.Agent(protocol, client);
            return agent;
        }

        public static Regulus.Remote.IAgent CreateTcp(System.Reflection.Assembly protocol_assembly)
        {
            var protocol = Regulus.Remote.Protocol.ProtocolProvider.Create(protocol_assembly);            
            return CreateTcp(protocol);
        }
        public static Regulus.Remote.IAgent CreateTcp(IProtocol protocol)
        {
            var client = new Regulus.Network.Tcp.ConnectProvider();
            var agent = new Regulus.Remote.Ghost.Agent(protocol, client);
            return agent;
        }
    }
}
