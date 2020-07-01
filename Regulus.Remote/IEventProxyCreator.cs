using System;

namespace Regulus.Remote
{
    public interface IEventProxyCreator
    {

        // todo 
        Delegate Create(long soul_id,int event_id , InvokeEventCallabck invoke_Event);
        Type GetType(); 
        string GetName();
    }
}