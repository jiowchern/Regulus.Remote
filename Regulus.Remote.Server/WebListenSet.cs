namespace Regulus.Remote.Server
{
    public class WebListenSet : ListenSet<Regulus.Remote.Server.Web.Listener, Soul.IService>
    {
        public WebListenSet(Regulus.Remote.Server.Web.Listener listener, Soul.IService service) : base(listener, service) { }
    }
}
