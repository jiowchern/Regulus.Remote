

namespace Regulus.Framework.Client.JIT
{
    public static class AgentProivder
    {
        
        public static Regulus.Remote.IAgent CreateRudp(System.Reflection.Assembly common_assembly)
        {
            var protocol = Regulus.Remote.Protocol.AssemblyBuilder.CreateProtocol(common_assembly);
            var client = new Regulus.Network.Rudp.Client(new Regulus.Network.Rudp.UdpSocket());
            var agent = new Regulus.Remote.Ghost.Agent(protocol , client);
            return agent;
        }

        public static Regulus.Remote.IAgent CreateTcp(System.Reflection.Assembly common_assembly)
        {
            var protocol = Regulus.Remote.Protocol.AssemblyBuilder.CreateProtocol(common_assembly);
            var client = new  Regulus.Network.Tcp.Client();
            var agent = new Regulus.Remote.Ghost.Agent(protocol, client);
            return agent;
        }

    }
}
