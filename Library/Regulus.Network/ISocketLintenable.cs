using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Network
{
    public interface ISocketLintenable
    {
        event Action<ISocket> AcceptEvent;
        void Bind(int port);
        void Close();
    }
}
