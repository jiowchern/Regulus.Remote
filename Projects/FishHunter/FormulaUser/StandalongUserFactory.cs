// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StandaloneUserFactory.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the StandaloneUserFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------



using System;

using Regulus.Framework;
using Regulus.Remoting;
using Regulus.Remoting.Standalone;
using Regulus.Utility;

using Console = Regulus.Utility.Console;

namespace VGame.Project.FishHunter.Formula
{
	public class StandaloneUserFactory : IUserFactoty<IUser>
	{
		private readonly ICore _Standalone;

		public StandaloneUserFactory(ICore core)
		{
			this._Standalone = core;

			if (this._Standalone == null)
			{
				throw new ArgumentNullException("Core is null");
			}
		}

		IUser IUserFactoty<IUser>.SpawnUser()
		{
			var agent = new Agent();
			agent.ConnectedEvent += () => { this._Standalone.AssignBinder(agent); };
			return new User(agent);
		}

		ICommandParsable<IUser> IUserFactoty<IUser>.SpawnParser(Command command, Console.IViewer view, IUser user)
		{
			return new CommandParser(command, view, user);
		}
	}
}