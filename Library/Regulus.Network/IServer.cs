using System;

namespace Regulus.Network
{
    public interface IServer
    {
        event Action<IPeer> AcceptEvent;
        void Bind(int Port);
        void Close();
    }
}
