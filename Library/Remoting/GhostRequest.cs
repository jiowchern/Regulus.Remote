using System.Collections.Generic;

namespace Regulus.Remoting
{
	public interface IGhostRequest
	{
		void Request(ClientToServerOpCode code, byte[] args);
	}
}
