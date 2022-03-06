namespace Regulus.Remote.Client
{
    public class TcpConnectSet : ConnectSet<Regulus.Network.Tcp.Connecter>
    {
        public TcpConnectSet(Regulus.Network.Tcp.Connecter connecter, Ghost.IAgent agent) : base(connecter, agent) { }
    }
}