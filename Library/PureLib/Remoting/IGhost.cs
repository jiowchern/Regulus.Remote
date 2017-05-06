using System;

namespace Regulus.Remoting
{
	public interface IGhost
	{
		void OnEvent(string name_event, byte[][] args);

		Guid GetID();

		void OnProperty(string name, object value);

		bool IsReturnType();
	}
}
