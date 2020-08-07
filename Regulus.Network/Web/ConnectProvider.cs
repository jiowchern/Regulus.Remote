using Regulus.Utility;

namespace Regulus.Network.Web
{
    public class ConnectProvider : IConnectProvidable
    {
        void IBootable.Launch()
        {

        }

        void IBootable.Shutdown()
        {

        }

        IConnectable IConnectProvidable.Spawn()
        {
            return new Connecter(new System.Net.WebSockets.ClientWebSocket());
        }
    }
}
