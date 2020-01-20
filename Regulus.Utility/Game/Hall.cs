/*using Regulus.Framework;
using Regulus.Utility;

namespace Regulus.Game
{
	public class Hall : IUpdatable
	{
		public event OnNewUser NewUserEvent;

		private readonly Updater _Users;

		public Hall()
		{
			_Users = new Updater();
		}

		void IBootable.Launch()
		{
		}

		bool IUpdatable.Update()
		{
			_Users.Working();
			return true;
		}

		void IBootable.Shutdown()
		{
			_Users.Shutdown();
		}

		public void PushUser(IUser user)
		{
			user.VerifySuccessEvent += id =>
			{
				if(NewUserEvent != null)
				{
					NewUserEvent(id);
				}

				NewUserEvent += user.OnKick;
			};

			user.QuitEvent += () =>
			{
				NewUserEvent -= user.OnKick;
				_Users.Remove(user);
			};
			_Users.Add(user);
		}
	}
}
*/