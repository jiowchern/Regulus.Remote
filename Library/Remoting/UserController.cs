// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserController.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the UserController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using Regulus.Framework;
using Regulus.Game;
using Regulus.Utility;

#endregion

namespace Regulus.Remoting
{
	public class UserController<TUser> : Framework<TUser>.IController
		where TUser : IUpdatable
	{
		public event OnLook LookEvent;

		public event OnLook UnlookEvent;

		private readonly Updater _Updater;

		private readonly TUser _User;

		public UserController(TUser user)
		{
			this._User = user;
			this._Updater = new Updater();
		}

		string Framework<TUser>.IController.Name { get; set; }

		void Framework<TUser>.IController.Look()
		{
			if (this.LookEvent != null)
			{
				this.LookEvent(this._User);
			}
		}

		void Framework<TUser>.IController.NotLook()
		{
			if (this.UnlookEvent != null)
			{
				this.UnlookEvent(this._User);
			}
		}

		bool IUpdatable.Update()
		{
			this._Updater.Working();
			return true;
		}

		void IBootable.Launch()
		{
			this._Updater.Add(this._User);
		}

		void IBootable.Shutdown()
		{
			this._Updater.Shutdown();
		}

		TUser Framework<TUser>.IController.GetUser()
		{
			return this._User;
		}

		public delegate void OnLook(TUser user);
	}
}