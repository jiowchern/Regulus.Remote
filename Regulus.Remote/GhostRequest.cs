using System.Collections.Generic;

namespace Regulus.Remote
{
	public interface IGhostRequest
	{
		void Request(ClientToServerOpCode code, byte[] args);
	}
}
