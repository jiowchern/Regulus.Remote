using System;

namespace Regulus.Remote.Soul
{
    public interface IService : IDisposable
    {
        void Join(Network.IStreamable stream);
        void Leave(Network.IStreamable stream);

    }
}
