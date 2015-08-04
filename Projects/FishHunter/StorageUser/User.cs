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
using VGame.Project.FishHunter.Common.GPIs;

#endregion

namespace VGame.Project.FishHunter.Storage
{
	internal class User : IUser
	{
		private readonly IAgent _Agent;

		private readonly Regulus.Remoting.User _Remoting;

		private readonly Updater _Updater;

		public User(IAgent agent)
		{
			this._Agent = agent;
			_Updater = new Updater();
			_Remoting = new Regulus.Remoting.User(agent);
		}

		Regulus.Remoting.User IUser.Remoting
		{
			get { return _Remoting; }
		}

		INotifier<IVerify> IUser.VerifyProvider
		{
			get { return _Agent.QueryNotifier<IVerify>(); }
		}

		bool IUpdatable.Update()
		{
			_Updater.Working();
			return true;
		}

		void IBootable.Launch()
		{
			_Updater.Add(_Agent);
			_Updater.Add(_Remoting);
		}

		void IBootable.Shutdown()
		{
			_Updater.Shutdown();
		}

		INotifier<T> IUser.QueryProvider<T>()
		{
			return _Agent.QueryNotifier<T>();
		}

		INotifier<IStorageCompetences> IUser.StorageCompetncesProvider
		{
			get { return _Agent.QueryNotifier<IStorageCompetences>(); }
		}
	}
}