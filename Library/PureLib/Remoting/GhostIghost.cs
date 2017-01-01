using System;

namespace Regulus.Remoting
{
	public interface IGhost
	{
		void OnEvent(string name_event, byte[][] args);

		Guid GetID();

		void OnProperty(string name, byte[] value);

		bool IsReturnType();
	}
}
