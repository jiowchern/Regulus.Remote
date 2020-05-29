using System;

namespace Regulus.Remote
{
    public interface IDirtyable
    {
        event Action<Type, object> DirtyEvent;
    }
}