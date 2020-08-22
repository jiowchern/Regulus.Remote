using System;

namespace Regulus.Remote.Soul
{
    public interface IService : IDisposable
    {
        void Join(Network.IStreamable stream,object state = null);
        void Leave(Network.IStreamable stream);

    }
}
