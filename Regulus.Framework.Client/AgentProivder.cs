using System.Linq;

namespace Regulus.Remote.Client
{
    
    public static class AgentProivder
    {        

        public static Regulus.Remote.IAgent CreateRudp(System.Reflection.Assembly protocol_assembly)
        {
            
            var protocol = Regulus.Remote.Protocol.ProtocolProvider.Create(protocol_assembly);
            var client = new Regulus.Network.Rudp.ConnectProvider(new Regulus.Network.Rudp.UdpSocket());
            var agent = new Regulus.Remote.Ghost.Agent(protocol, client);
            return agent;
        }

        public static Regulus.Remote.IAgent CreateTcp(System.Reflection.Assembly protocol_assembly)
        {
            var protocol = Regulus.Remote.Protocol.ProtocolProvider.Create(protocol_assembly);
            var client = new Regulus.Network.Tcp.ConnectProvider();
            var agent = new Regulus.Remote.Ghost.Agent(protocol, client);
            return agent;
        }
    }
}
namespace Regulus.Remote.Client.JIT
{
    public static class AgentProivder
    {
        
        public static Regulus.Remote.IAgent CreateRudp(params System.Type[] types)
        {
            var client = new Regulus.Network.Rudp.ConnectProvider(new Regulus.Network.Rudp.UdpSocket());

            
            var ab = new Regulus.Remote.Protocol.AssemblyBuilder(types);
            var protocolAsm = ab.Create();
            var protocol = Regulus.Remote.Protocol.ProtocolProvider.Create(protocolAsm);
            var agent = new Regulus.Remote.Ghost.Agent(protocol , client);
            return agent;
        }

        public static Regulus.Remote.IAgent CreateTcp(params System.Type[] types)
        {
            
            var client = new  Regulus.Network.Tcp.ConnectProvider();
            
            var ab = new Regulus.Remote.Protocol.AssemblyBuilder(types);
            var protocolAsm = ab.Create();
            var protocol = Regulus.Remote.Protocol.ProtocolProvider.Create(protocolAsm);
            var agent = new Regulus.Remote.Ghost.Agent(protocol, client);
            return agent;
        }

    }
}
