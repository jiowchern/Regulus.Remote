using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Game
{
	public delegate void OnNewUser(Guid account);
	public delegate void OnQuit();
    public delegate void DoneCallback();
	public interface IUser : Regulus.Utility.IUpdatable
	{
		void OnKick(Guid id);
		event OnNewUser VerifySuccessEvent;
		event OnQuit QuitEvent;
	}
}
