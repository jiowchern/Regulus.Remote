using Regulus.Framework;
using Regulus.Remote;

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
