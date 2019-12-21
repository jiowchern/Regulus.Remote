using System;

namespace Regulus.Remote
{
    public interface IEventProxyCreator
    {
        Delegate Create(Guid soul_id,int event_id , InvokeEventCallabck invoke_Event);
        Type GetType(); 
        string GetName();
    }
}