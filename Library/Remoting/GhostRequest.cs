using System.Collections.Generic;

namespace Regulus.Remoting
{
	public interface IGhostRequest
	{
		void Request(byte code, Dictionary<byte, byte[]> args);
	}
}
