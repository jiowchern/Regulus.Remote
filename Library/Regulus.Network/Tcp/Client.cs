using System;
using Regulus.Network;

namespace Regulus.Network.Tcp
{
    public class Client : IClient
    {
        

        IConnectable IClient.Spawn()
        {
            return new Connecter();
        }
    }
}