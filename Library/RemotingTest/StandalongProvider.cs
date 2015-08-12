// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StandaloneProvider.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the StandaloneProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using Regulus.Framework;
using Regulus.Remoting;
using Regulus.Remoting.Standalone;
using Regulus.Utility;

#endregion

namespace RemotingTest
{
	public class StandaloneProvider : IUserFactoty<IUser>
	{
		private readonly ICore _Standalone;

		public StandaloneProvider(ICore core)
		{
			_Standalone = core;
		}

		IUser IUserFactoty<IUser>.SpawnUser()
		{
			var agent = new Agent();
			agent.ConnectedEvent += () => { _Standalone.AssignBinder(agent); };
			return new User(agent);
		}

		ICommandParsable<IUser> IUserFactoty<IUser>.SpawnParser(Command command, Console.IViewer view, IUser user)
		{
			return new CommandParser();
		}
	}
}