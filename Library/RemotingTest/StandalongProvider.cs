<<<<<<< HEAD
﻿using Regulus.Framework;
=======
﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StandaloneProvider.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the StandaloneProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using Regulus.Framework;
>>>>>>> bb08c0b8a8aa5ec0c708cd9f624c302cd192eb5d
using Regulus.Remoting;
using Regulus.Remoting.Standalone;
using Regulus.Utility;

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
