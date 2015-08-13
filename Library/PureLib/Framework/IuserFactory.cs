using Regulus.Utility;

namespace Regulus.Framework
{
	public interface IUserFactoty<TUser>
	{
		TUser SpawnUser();

		ICommandParsable<TUser> SpawnParser(Command command, Console.IViewer view, TUser user);
	}
}
