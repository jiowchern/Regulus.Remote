namespace Regulus.Remote.Client
{
    public class TcpIntegral : Integral<Regulus.Network.Tcp.Connecter>
    {
        public TcpIntegral(Regulus.Network.Tcp.Connecter connecter, Ghost.IAgent agent) : base(connecter, agent) { }
    }
}