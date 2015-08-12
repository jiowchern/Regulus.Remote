// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StandaloneFactory.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the StandaloneFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using Regulus.Framework;
using Regulus.Remoting;
using Regulus.Remoting.Standalone;
using Regulus.Utility;

#endregion

namespace VGame.Project.FishHunter.Storage
{
	public class StandaloneFactory : IUserFactoty<IUser>
	{
		private readonly ICore _Core;

		public StandaloneFactory(ICore core)
		{
			_Core = core;
		}

		IUser IUserFactoty<IUser>.SpawnUser()
		{
			var agent = new Agent();
			agent.ConnectedEvent += () => { _Core.AssignBinder(agent); };
			return new User(agent);
		}

		ICommandParsable<IUser> IUserFactoty<IUser>.SpawnParser(Command command, Console.IViewer view, IUser user)
		{
			return new CommandParser(command, view, user);
		}
	}
}