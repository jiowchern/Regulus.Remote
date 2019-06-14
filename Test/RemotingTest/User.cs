using Regulus.Framework;
using Regulus.Remote;
using Regulus.Utility;

namespace RemotingTest
{
	internal class User : IUser
	{
		private readonly IAgent _Agent;

		private readonly Updater _Updater;

		private readonly Regulus.Remote.User _User;

		public User(IAgent agent)
		{
			_Agent = agent;
			_Updater = new Updater();
			_User = new Regulus.Remote.User(_Agent);
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

		Regulus.Remote.User IUser.Remoting
		{
			get { return _User; }
		}

		INotifier<ITestReturn> IUser.TestReturnProvider
		{
			get { return _Agent.QueryNotifier<ITestReturn>(); }
		}
	}
}
