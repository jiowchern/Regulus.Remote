using System.Collections.Generic;

namespace Regulus.Remoting
{
	public interface IResponseQueue
	{
		void Push(byte cmd, Dictionary<byte, byte[]> args);
	}
}
