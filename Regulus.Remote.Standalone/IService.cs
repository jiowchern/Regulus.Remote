using Regulus.Remote.Ghost;
using System;

namespace Regulus.Remote.Standalone
{
    public interface IService : IDisposable
    {
        void Join(IAgent agent);
        void Leave(IAgent agent);
        
    }
}
