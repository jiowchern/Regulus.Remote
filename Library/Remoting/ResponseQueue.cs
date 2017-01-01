using System.Collections.Generic;

namespace Regulus.Remoting
{
	public interface IResponseQueue
	{
		void Push(ServerToClientOpCode code, byte[] data);
	}
}
