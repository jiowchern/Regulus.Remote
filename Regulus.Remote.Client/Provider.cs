namespace Regulus.Remote.Client
{
    public class Provider
    {
        public static Regulus.Remote.Client.TcpSocket CreateTcp(Ghost.IAgent agent)
        {
            
            return new Regulus.Remote.Client.TcpSocket(agent);
        }
        public static Ghost.IAgent CreateAgent(IProtocol protocol)
        {
            return new Ghost.Agent(protocol);
        }
    }
}