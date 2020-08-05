using Regulus.Utility;
using System;
using System.Linq;

namespace Regulus.Remote.Client
{
    
    public  class AgentProvider : IAgentProvider
    {
        

        public static Regulus.Remote.IAgent CreateRudp(System.Reflection.Assembly protocol_assembly)
        {
            
            var protocol = Regulus.Remote.Protocol.ProtocolProvider.Create(protocol_assembly);            
            return CreateRudp(protocol);
        }

        public static Regulus.Remote.IAgent CreateRudp(IProtocol protocol)
        {
            // todo
            throw new NotImplementedException();
            /*var client = new Regulus.Network.Rudp.ConnectProvider(new Regulus.Network.Rudp.UdpSocket());
            var agent = new Regulus.Remote.Ghost.Agent(protocol, client);
            return agent;*/
        }

        public static Regulus.Remote.IAgent Create(IProtocol protocol, Regulus.Network.IConnectProvidable connect_providable)
        {
            // todo
            throw new NotImplementedException();
            /*var agent = new Regulus.Remote.Ghost.Agent(protocol, connect_providable);
            return agent;*/
        }



        public static Regulus.Remote.IAgent CreateTcp(System.Reflection.Assembly protocol_assembly)
        {
            var protocol = Regulus.Remote.Protocol.ProtocolProvider.Create(protocol_assembly);            
            return CreateTcp(protocol);
        }

        public static IAgent CreateStandalone(IProtocol arg,Remote.IBinderProvider entry)
        {
            // todo
            throw new NotImplementedException();
            /*var agent = new Regulus.Remote.Standalone.Agent(arg);
            (agent as IAgent).ConnectEvent += () => {

                var binder = agent as IBinder;
                entry.AssignBinder(binder);
            }; 
            
            return agent;*/
        }

        public static Regulus.Remote.IAgent CreateTcp(IProtocol protocol)
        {
            // todo
            throw new NotImplementedException();
            /*var client = new Regulus.Network.Tcp.ConnectProvider();
            var agent = new Regulus.Remote.Ghost.Agent(protocol, client);
            return agent;*/
        }

        public static Regulus.Remote.IAgent CreateWeb(IProtocol protocol)
        {
            // todo
            throw new NotImplementedException();
            /*var client = new Regulus.Network.Web.ConnectProvider();
            var agent = new Regulus.Remote.Ghost.Agent(protocol, client);
            return agent;*/
        }

        private readonly IProtocol protocol;
        private readonly Func<IProtocol, IAgent> func;

        public AgentProvider(IProtocol protocol, System.Func<IProtocol, IAgent> func)
        {
            this.protocol = protocol;
            this.func = func;
        }
        IAgent IAgentProvider.Spawn()
        {
            return func(protocol);
        }
    }


    
}
