namespace Regulus.Remote.Server
{
    public class TcpIntegral : Integral<Regulus.Remote.Server.Tcp.Listener>
    {
        public TcpIntegral(Regulus.Remote.Server.Tcp.Listener listener, Soul.IService service) : base(listener ,service) { }
    }
}
