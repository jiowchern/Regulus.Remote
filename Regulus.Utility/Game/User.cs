using System;


using Regulus.Utility;

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
