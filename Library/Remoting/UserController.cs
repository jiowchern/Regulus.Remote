using Regulus.Framework;
using Regulus.Game;
using Regulus.Utility;

namespace Regulus.Remoting
{
	public class UserController<TUser> : Framework<TUser>.IController
		where TUser : IUpdatable
	{
		public delegate void OnLook(TUser user);

		public event OnLook LookEvent;

		public event OnLook UnlookEvent;

		private readonly Updater _Updater;

		private readonly TUser _User;

		public UserController(TUser user)
		{
			_User = user;
			_Updater = new Updater();
		}

		string Framework<TUser>.IController.Name { get; set; }

		void Framework<TUser>.IController.Look()
		{
			if(LookEvent != null)
			{
				LookEvent(_User);
			}
		}

		void Framework<TUser>.IController.NotLook()
		{
			if(UnlookEvent != null)
			{
				UnlookEvent(_User);
			}
		}

		bool IUpdatable.Update()
		{
			_Updater.Working();
			return true;
		}

		void IBootable.Launch()
		{
			_Updater.Add(_User);
		}

		void IBootable.Shutdown()
		{
			_Updater.Shutdown();
		}

		TUser Framework<TUser>.IController.GetUser()
		{
			return _User;
		}
	}
}
