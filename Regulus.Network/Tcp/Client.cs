using System;
using Regulus.Framework;
using Regulus.Network;

namespace Regulus.Network.Tcp
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
            return new Connecter();
        }
    }
}