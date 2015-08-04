// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IuserFactory.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the IUserFactoty type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using Regulus.Utility;

#endregion

namespace Regulus.Framework
{
	public interface IUserFactoty<TUser>
	{
		TUser SpawnUser();

		ICommandParsable<TUser> SpawnParser(Command command, Console.IViewer view, TUser user);
	}
}