using System;

namespace Regulus.Network
{
    public interface IListenable
    {
        event Action<IStreamable> AcceptEvent;
        void Bind(int port);
        void Close();
    }
}
