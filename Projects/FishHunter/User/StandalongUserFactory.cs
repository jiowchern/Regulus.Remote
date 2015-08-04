// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StandalongUserFactory.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the StandalongUserFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using Regulus.Framework;
using Regulus.Remoting;
using Regulus.Remoting.Standalong;
using Regulus.Utility;

#endregion

namespace VGame.Project.FishHunter
{
	public class StandalongUserFactory
		: IUserFactoty<IUser>
	{
		private readonly ICore _Standalong;

		public StandalongUserFactory(ICore core)
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
			return new CommandParser(command, view, user);
		}
	}
}