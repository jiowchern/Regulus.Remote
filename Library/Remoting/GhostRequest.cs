using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Remoting
{
	public interface IGhostRequest
	{
		void Request(byte code , Dictionary<byte , object> args );
	}
}
