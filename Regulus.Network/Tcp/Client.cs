using System;
using Regulus.Utility;
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