using Regulus.Framework;
using Regulus.Remoting.Ghost.Native;
using Regulus.Utility;

namespace VGame.Project.FishHunter.Storage
{
	public class RemotingFactory : IUserFactoty<IUser>
	{
		IUser IUserFactoty<IUser>.SpawnUser()
		{
			return new User(Agent.Create());
		}

		ICommandParsable<IUser> IUserFactoty<IUser>.SpawnParser(Command command, Console.IViewer view, IUser user)
		{
			return new CommandParser(command, view, user);
		}
	}
}
