// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StandalongUserFactory.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the StandalongUserFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------



using System;

using Regulus.Framework;
using Regulus.Remoting;
using Regulus.Remoting.Standalong;
using Regulus.Utility;

using Console = Regulus.Utility.Console;

namespace VGame.Project.FishHunter.Formula
{
	public class StandalongUserFactory : IUserFactoty<IUser>
	{
		private readonly ICore _Standalong;

		public StandalongUserFactory(ICore core)
		{
			this._Standalong = core;

			if (this._Standalong == null)
			{
				throw new ArgumentNullException("Core is null");
			}
		}

		IUser IUserFactoty<IUser>.SpawnUser()
		{
			var agent = new Agent();
			agent.ConnectedEvent += () => { this._Standalong.AssignBinder(agent); };
			return new User(agent);
		}

		ICommandParsable<IUser> IUserFactoty<IUser>.SpawnParser(Command command, Console.IViewer view, IUser user)
		{
			return new CommandParser(command, view, user);
		}
	}
}