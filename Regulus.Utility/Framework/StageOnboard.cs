using Regulus.Utility;

namespace Regulus.Framework
{
	internal class OnBoard<TUser> : IStatus
		where TUser : class, IUpdatable
	{
		public delegate void OnDone();

		public event OnDone DoneEvent;

		private readonly Command _Command;

		private readonly Updater _Updater;

		private readonly UserProvider<TUser> _UserProvider;

		public OnBoard(UserProvider<TUser> user_provider, Command command)
		{
			_Updater = new Updater();
			_UserProvider = user_provider;
			_Command = command;
		}

		void IStatus.Enter()
		{
			_Updater.Add(_UserProvider);
			_Command.Register<string>("SpawnUser[UserName]", _Spawn);
			_Command.Register<string>("UnpawnUser[UserName]", _Unspawn);
			_Command.Register<string>("SelectUser[UserName]", _Select);
		}

		void IStatus.Leave()
		{
			_Command.Unregister("SelectUser");
			_Command.Unregister("SpawnUser");
			_Command.Unregister("UnpawnUser");
			_Updater.Shutdown();
		}

		void IStatus.Update()
		{
			_Updater.Working();
		}

		private void _Spawn(string name)
		{
			_UserProvider.Spawn(name);
		}

		private void _Unspawn(string name)
		{
			_UserProvider.Unspawn(name);
		}

		private void _Select(string name)
		{
			_UserProvider.Select(name);
		}
	}
}
