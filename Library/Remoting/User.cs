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
using Regulus.Utility;

#endregion

namespace Regulus.Remoting
{
	public class User : IUpdatable
	{
		private readonly IAgent _Agent;

		private readonly TProvider<IConnect> _ConnectProvider;

		private readonly StageMachine _Machine;

		private readonly TProvider<IOnline> _OnlineProvider;

		private readonly Updater _Updater;

		public INotifier<IConnect> ConnectProvider
		{
			get { return _ConnectProvider; }
		}

		public INotifier<IOnline> OnlineProvider
		{
			get { return _OnlineProvider; }
		}

		public User(IAgent agent)
		{
			_Agent = agent;
			_ConnectProvider = new TProvider<IConnect>();
			_OnlineProvider = new TProvider<IOnline>();
			_Machine = new StageMachine();
			_Updater = new Updater();
		}

		bool IUpdatable.Update()
		{
			_Updater.Working();
			_Machine.Update();
			return true;
		}

		void IBootable.Launch()
		{
			_Updater.Add(_Agent);
			_ToOffline();
		}

		void IBootable.Shutdown()
		{
			_Machine.Termination();
			_Updater.Shutdown();
		}

		private void _ToOffline()
		{
			var stage = new OfflineStage(_Agent, _ConnectProvider);

			stage.DoneEvent += _ToOnline;

			_Machine.Push(stage);
		}

		private void _ToOnline()
		{
			var stage = new OnlineStage(_Agent, _OnlineProvider);

			stage.BreakEvent += _ToOffline;

			_Machine.Push(stage);
		}
	}
}