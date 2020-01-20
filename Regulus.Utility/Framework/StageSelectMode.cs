using System;


using Regulus.Utility;

namespace Regulus.Framework
{
	internal class SelectMode<TUser> : IStatus
		where TUser : class, IUpdatable
	{
		public delegate void OnDone<TUser>(UserProvider<TUser> console) where TUser : class, IUpdatable;

		public event OnDone<TUser> DoneEvent;

		public event Action InitialedEvent;

		private readonly Command _Command;

		private readonly GameModeSelector<TUser> _Selector;

		public SelectMode(GameModeSelector<TUser> mode_selector, Command command)
		{
			_Command = command;
			_Selector = mode_selector;
		}

		void IStatus.Enter()
		{
			_Selector.GameConsoleEvent += _ObtainConsole;
			_Command.Register<string>("CreateMode", _CreateGameConsole);

			InitialedEvent();
		}

		void IStatus.Leave()
		{
			_Command.Unregister("CreateMode");
			_Selector.GameConsoleEvent -= _ObtainConsole;
		}

		void IStatus.Update()
		{
		}

		private void _ObtainConsole(UserProvider<TUser> console)
		{
			DoneEvent(console);
		}

		private void _CreateGameConsole(string name)
		{
			_Selector.CreateUserProvider(name);
		}
	}
}
