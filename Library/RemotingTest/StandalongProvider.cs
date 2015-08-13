using Regulus.Framework;
using Regulus.Remoting;
using Regulus.Remoting.Standalong;
using Regulus.Utility;

namespace RemotingTest
{
	public class StandalongProvider : IUserFactoty<IUser>
	{
		private readonly ICore _Standalong;

		public StandalongProvider(ICore core)
		{
			_Standalong = core;
		}

		IUser IUserFactoty<IUser>.SpawnUser()
		{
			var agent = new Agent();
			agent.ConnectedEvent += () => { _Standalong.AssignBinder(agent); };
			return new User(agent);
		}

		ICommandParsable<IUser> IUserFactoty<IUser>.SpawnParser(Command command, Console.IViewer view, IUser user)
		{
			return new CommandParser();
		}
	}
}
