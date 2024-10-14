namespace Regulus.Remote.Client
{
    public class TcpConnectSet : ConnectSet<Regulus.Network.Tcp.Connector>
    {
        public TcpConnectSet(Regulus.Network.Tcp.Connector connecter, Ghost.IAgent agent) : base(connecter, agent) { }
    }
}