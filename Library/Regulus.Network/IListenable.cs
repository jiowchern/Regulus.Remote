using System;

namespace Regulus.Network
{
    public interface IListenable
    {
        event Action<IPeer> AcceptEvent;
        void Bind(int Port);
        void Close();
    }
}
