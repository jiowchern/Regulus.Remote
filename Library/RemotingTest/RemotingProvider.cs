// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemotingProvider.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the RemotingProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using Regulus.Framework;
using Regulus.Remoting.Ghost.Native;
using Regulus.Utility;

#endregion

namespace RemotingTest
{
	internal class RemotingProvider : IUserFactoty<IUser>
	{
		IUser IUserFactoty<IUser>.SpawnUser()
		{
			return new User(Agent.Create());
		}

		ICommandParsable<IUser> IUserFactoty<IUser>.SpawnParser(Command command, Console.IViewer view, IUser user)
		{
			return new CommandParser();
		}
	}
}