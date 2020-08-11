using System;

namespace Regulus.Remote
{
    public interface IEventProxyCreator
    {

        
        Delegate Create(long soul_id, int event_id, long handler_id, InvokeEventCallabck invoke_Event);
        Type GetType();
        string GetName();
    }
}