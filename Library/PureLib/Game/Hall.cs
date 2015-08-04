// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Hall.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the Hall type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using Regulus.Framework;
using Regulus.Utility;

#endregion

namespace Regulus.Game
{
	public class Hall : IUpdatable
	{
		public event OnNewUser NewUserEvent;

		private readonly Updater _Users;

		public Hall()
		{
			this._Users = new Updater();
		}

		void IBootable.Launch()
		{
		}

		bool IUpdatable.Update()
		{
			this._Users.Working();
			return true;
		}

		void IBootable.Shutdown()
		{
			this._Users.Shutdown();
		}

		public void PushUser(IUser user)
		{
			user.VerifySuccessEvent += id =>
			{
				if (this.NewUserEvent != null)
				{
					this.NewUserEvent(id);
				}

				this.NewUserEvent += user.OnKick;
			};

			user.QuitEvent += () =>
			{
				this.NewUserEvent -= user.OnKick;
				this._Users.Remove(user);
			};
			this._Users.Add(user);
		}
	}
}