using System;
using Regulus.Network;

namespace Regulus.Network.Tcp
{
    public class ConnectProvider : IConnectProviderable
    {
        

        IConnectable IConnectProviderable.Spawn()
        {
            return new Connecter();
        }
    }
}