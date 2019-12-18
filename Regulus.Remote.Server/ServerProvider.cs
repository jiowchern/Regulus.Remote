using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regulus.Remote.Server
{
    public static class ServerProvider
    {
        public static Regulus.Remote.Soul.Service CreateTcp(int port,IEntry entry,Regulus.Remote.Protocol.Essential essential)
        {
            var ab = new Regulus.Remote.Protocol.AssemblyBuilder(essential);
            var protocolAsm = ab.Create();
            var protocol = Regulus.Remote.Protocol.ProtocolProvider.Create(protocolAsm);

            return new Regulus.Remote.Soul.Service(entry, port, protocol, new Regulus.Network.Tcp.Listener());
        }

        public static Regulus.Remote.Soul.Service CreateRudp(int port, IEntry entry, Regulus.Remote.Protocol.Essential essential)
        {
            var ab = new Regulus.Remote.Protocol.AssemblyBuilder(essential);
            var protocolAsm = ab.Create();
            var protocol = Regulus.Remote.Protocol.ProtocolProvider.Create(protocolAsm);

            return new Regulus.Remote.Soul.Service(entry, port, protocol, new Regulus.Network.Rudp.Listener(new Regulus.Network.Rudp.UdpSocket()));
        }
    }
}
