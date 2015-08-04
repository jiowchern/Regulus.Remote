// --------------------------------------------------------------------------------------------------------------------
// <copyright file="User.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the User type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using Regulus.Framework;
using Regulus.Remoting;
using Regulus.Utility;

using VGame.Project.FishHunter.Common;
using VGame.Project.FishHunter.Common.GPI;

#endregion

namespace VGame.Project.FishHunter
{
	internal class User : IUser
	{
		private readonly IAgent _Agent;

		private readonly Updater _Updater;

		private readonly Regulus.Remoting.User _User;

		public User(IAgent agent)
		{
			this._Agent = agent;
			_Updater = new Updater();
			_User = new Regulus.Remoting.User(_Agent);
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

		Regulus.Remoting.User IUser.Remoting
		{
			get { return _User; }
		}

		INotifier<IVerify> IUser.VerifyProvider
		{
			get { return _Agent.QueryNotifier<IVerify>(); }
		}

		INotifier<IPlayer> IUser.PlayerProvider
		{
			get { return _Agent.QueryNotifier<IPlayer>(); }
		}

		INotifier<IAccountStatus> IUser.AccountStatusProvider
		{
			get { return _Agent.QueryNotifier<IAccountStatus>(); }
		}

		INotifier<ILevelSelector> IUser.LevelSelectorProvider
		{
			get { return _Agent.QueryNotifier<ILevelSelector>(); }
		}
	}
}