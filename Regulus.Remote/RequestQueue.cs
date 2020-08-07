using System;

namespace Regulus.Remote
{
    public delegate void InvokeMethodCallback(long entity_id, int method_id, long return_id, byte[][] args);

    public interface IRequestQueue
    {
        event Action BreakEvent;

        event InvokeMethodCallback InvokeMethodEvent;


    }
}
