// --------------------------------------------------------------------------------------------------------------------
// <copyright file="User.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the OnNewUser type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System;

using Regulus.Utility;

#endregion

namespace Regulus.Game
{
	public delegate void OnNewUser(Guid account);

	public delegate void OnQuit();

	public delegate void DoneCallback();

	public interface IUser : IUpdatable
	{
		event OnQuit QuitEvent;

		event OnNewUser VerifySuccessEvent;

		void OnKick(Guid id);
	}
}