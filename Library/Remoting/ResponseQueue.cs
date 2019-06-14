using System.Collections.Generic;

namespace Regulus.Remote
{
	public interface IResponseQueue
	{
		void Push(ServerToClientOpCode code, byte[] data);
	}
}
