// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StandaloneUserFactory.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the StandaloneUserFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using Regulus.Framework;
using Regulus.Remoting;
using Regulus.Remoting.Standalone;
using Regulus.Utility;

#endregion

namespace VGame.Project.FishHunter
{
	public class StandaloneUserFactory
		: IUserFactoty<IUser>
	{
		private readonly ICore _Standalone;

		public StandaloneUserFactory(ICore core)
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
			return new CommandParser(command, view, user);
		}
	}
}