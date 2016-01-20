using System;

namespace Regulus.Remoting
{
    public interface IGhostProvider
    {
        Type Find(Type ghost_base_type);
    }
}