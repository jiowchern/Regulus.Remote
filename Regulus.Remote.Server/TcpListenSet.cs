namespace Regulus.Remote.Server
{
    public class TcpListenSet : ListenSet<Regulus.Remote.Server.Tcp.Listener , Soul.IService>
    {
        public TcpListenSet(Regulus.Remote.Server.Tcp.Listener listener, Soul.IService service) : base(listener ,service) { }
    }
}
