using Regulus.Network;
using Regulus.Remote.Ghost;
using System;

namespace Regulus.Remote.Client
{

    public class Provider
    {        
        public static Ghost.Agent CreateAgent(IProtocol protocol, ISerializable serializable , Regulus.Memorys.IPool pool)
        {
            return new Ghost.Agent(protocol, serializable , new Regulus.Remote.InternalSerializer(), pool);
        }

        public static Ghost.Agent CreateAgent(IProtocol protocol)
        {
            return new Ghost.Agent( protocol, new Regulus.Remote.Serializer(protocol.SerializeTypes) , new Regulus.Remote.InternalSerializer() , Regulus.Memorys.PoolProvider.Shared);
        }

        public static TcpConnectSet CreateTcpAgent(IProtocol protocol, ISerializable serializable, Regulus.Memorys.IPool pool)
        {
            var connecter = new Regulus.Network.Tcp.Connector();
            
            var agent = CreateAgent(protocol, serializable , pool);
            
            return new TcpConnectSet(connecter, agent);
        }

        public static TcpConnectSet CreateTcpAgent(IProtocol protocol)
        {
            var connecter = new Regulus.Network.Tcp.Connector();
            var agent = CreateAgent(protocol,  new Regulus.Remote.Serializer(protocol.SerializeTypes), Regulus.Memorys.PoolProvider.Shared);            
            return new TcpConnectSet(connecter, agent);
        }
    }
}