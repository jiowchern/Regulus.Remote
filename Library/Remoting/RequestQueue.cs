using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Remoting
{
	public interface IRequestQueue	
	{
		event Action<Guid, string, Guid, byte[][]> InvokeMethodEvent;
		event Action BreakEvent;

		void Update();
	}
}
