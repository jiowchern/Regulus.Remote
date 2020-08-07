using Regulus.Utility;

namespace Regulus.Network.Tcp
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
            return new Connecter();
        }
    }
}