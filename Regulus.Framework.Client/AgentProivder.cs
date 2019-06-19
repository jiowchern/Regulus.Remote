using System.Linq;

namespace Regulus.Framework.Client
{
    public static class AgentProivder
    {
        public static Regulus.Remote.IProtocol CreateProtocol(System.Reflection.Assembly protocol_assembly)
        {
            var types = protocol_assembly.GetExportedTypes();
            var protocolType = types.Where(type => type.GetInterface(nameof(Regulus.Remote.IProtocol)) != null).FirstOrDefault();
            if (protocolType == null)
                throw new System.Exception($"找不到{nameof(Regulus.Remote.IProtocol)}的實作");
            return System.Activator.CreateInstance(protocolType) as Regulus.Remote.IProtocol;

        }

        public static Regulus.Remote.IAgent CreateRudp(System.Reflection.Assembly protocol_assembly)
        {
            
            var protocol = CreateProtocol(protocol_assembly);
            var client = new Regulus.Network.Rudp.Client(new Regulus.Network.Rudp.UdpSocket());
            var agent = new Regulus.Remote.Ghost.Agent(protocol, client);
            return agent;
        }

        public static Regulus.Remote.IAgent CreateTcp(System.Reflection.Assembly protocol_assembly)
        {
            var protocol = CreateProtocol(protocol_assembly);
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
            var libAsm = _FindAssembly("Regulus.Library.dll");
            var libAsm = _FindAssembly("Regulus.Remote.dll");
            var libAsm = _FindAssembly("Regulus.Library.dll");
            var ab = new Regulus.Remote.Protocol.AssemblyBuilder(common_assembly);
            var protocolAsm = ab.Create();
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
