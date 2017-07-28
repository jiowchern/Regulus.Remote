using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Network
{
    public interface IPeerServer
    {
        event Action<IPeer> AcceptEvent;
        void Bind(int port);
        void Close();
    }
}
