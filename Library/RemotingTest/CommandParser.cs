using Regulus.Framework;
using Regulus.Remoting;

namespace RemotingTest
{
	internal class CommandParser : ICommandParsable<IUser>
	{
		void ICommandParsable<IUser>.Setup(IGPIBinderFactory build)
		{
		}

		void ICommandParsable<IUser>.Clear()
		{
		}
	}
}
