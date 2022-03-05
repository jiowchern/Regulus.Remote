namespace Regulus.Remote.Server
{
    public class WebIntegral : Integral<Regulus.Remote.Server.Web.Listener>
    {
        public WebIntegral(Regulus.Remote.Server.Web.Listener listener, Soul.IService service) : base(listener, service) { }
    }
}
