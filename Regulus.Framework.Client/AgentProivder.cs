using System.Linq;

namespace Regulus.Framework.Client
{
    
    public static class AgentProivder
    {        

        public static Regulus.Remote.IAgent CreateRudp(System.Reflection.Assembly protocol_assembly)
        {
            
            var protocol = Regulus.Remote.Protocol.ProtocolProvider.Create(protocol_assembly);
            var client = new Regulus.Network.Rudp.Client(new Regulus.Network.Rudp.UdpSocket());
            var agent = new Regulus.Remote.Ghost.Agent(protocol, client);
            return agent;
        }

        public static Regulus.Remote.IAgent CreateTcp(System.Reflection.Assembly protocol_assembly)
        {
            var protocol = Regulus.Remote.Protocol.ProtocolProvider.Create(protocol_assembly);
            var client = new Regulus.Network.Tcp.Client();
            var agent = new Regulus.Remote.Ghost.Agent(protocol, client);
            return agent;
        }
    }
}
namespace Regulus.Framework.Client.JIT
{
    public static class AgentProivder
    {
        
        public static Regulus.Remote.IAgent CreateRudp(System.Reflection.Assembly common_assembly)
        {
            var client = new Regulus.Network.Rudp.Client(new Regulus.Network.Rudp.UdpSocket());

            
            var ab = new Regulus.Remote.Protocol.AssemblyBuilder(common_assembly);
            var protocolAsm = ab.Create();
            var protocol = Regulus.Remote.Protocol.ProtocolProvider.Create(protocolAsm);
            var agent = new Regulus.Remote.Ghost.Agent(protocol , client);
            return agent;
        }

        public static Regulus.Remote.IAgent CreateTcp(System.Reflection.Assembly common_assembly)
        {
            
            var client = new  Regulus.Network.Tcp.Client();
            
            var ab = new Regulus.Remote.Protocol.AssemblyBuilder(common_assembly);
            var protocolAsm = ab.Create();
            var protocol = Regulus.Remote.Protocol.ProtocolProvider.Create(protocolAsm);
            var agent = new Regulus.Remote.Ghost.Agent(protocol, client);
            return agent;
        }

    }
}
