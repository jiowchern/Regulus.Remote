using System;

namespace Regulus.Remote
{
    public delegate void InvokeMethodCallback(Guid entity_id, int method_id, Guid return_id, byte[][] args);

	public interface IRequestQueue
	{
		event Action BreakEvent;

		event InvokeMethodCallback InvokeMethodEvent;

		
	}
}
