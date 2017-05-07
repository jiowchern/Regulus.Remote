using System;

namespace Regulus.Remoting
{
    public interface IEventProxyCreator
    {
        Delegate Create(Guid soul_id, Action<Guid, string, object[]> invoke_Event);
        Type GetType(); 
        string GetName();
    }
}