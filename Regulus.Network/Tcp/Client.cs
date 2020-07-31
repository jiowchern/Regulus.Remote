using System;
using Regulus.Utiliey;
using Regulus.Network;

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