using System;

namespace Regulus.Remoting
{
	public interface IRequestQueue
	{
		event Action BreakEvent;

		event Action<Guid, string, Guid, byte[][]> InvokeMethodEvent;

		void Update();
	}
}
