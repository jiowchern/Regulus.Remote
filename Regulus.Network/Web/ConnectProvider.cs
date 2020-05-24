using Regulus.Framework;

namespace Regulus.Network.Web
{
    public class ConnectProvider : IConnectProviderable
    {
        void IBootable.Launch()
        {
            
        }

        void IBootable.Shutdown()
        {
            
        }

        IConnectable IConnectProviderable.Spawn()
        {
            return new Connecter(new System.Net.WebSockets.ClientWebSocket());
        }
    }
}
