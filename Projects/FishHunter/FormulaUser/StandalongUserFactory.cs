<<<<<<< HEAD
﻿using System;
=======
﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StandaloneUserFactory.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the StandaloneUserFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
>>>>>>> bb08c0b8a8aa5ec0c708cd9f624c302cd192eb5d


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
<<<<<<< HEAD
			_Standalong = core;

			if(_Standalong == null)
=======
			this._Standalone = core;

			if (this._Standalone == null)
>>>>>>> bb08c0b8a8aa5ec0c708cd9f624c302cd192eb5d
			{
				throw new ArgumentNullException("Core is null");
			}
		}

		IUser IUserFactoty<IUser>.SpawnUser()
		{
			var agent = new Agent();
<<<<<<< HEAD
			agent.ConnectedEvent += () => { _Standalong.AssignBinder(agent); };
=======
			agent.ConnectedEvent += () => { this._Standalone.AssignBinder(agent); };
>>>>>>> bb08c0b8a8aa5ec0c708cd9f624c302cd192eb5d
			return new User(agent);
		}

		ICommandParsable<IUser> IUserFactoty<IUser>.SpawnParser(Command command, Console.IViewer view, IUser user)
		{
			return new CommandParser(command, view, user);
		}
	}
}
