using System;
using System.Collections.Generic;
using System.Linq;


using Regulus.Utility;


using Console = Regulus.Utility.Console;

namespace Regulus.Framework
{
	public class GameModeSelector<TUser>
		where TUser : class, IUpdatable
	{
		public delegate void OnGameConsole(UserProvider<TUser> console);

		private event OnGameConsole _GameConsoleEvent;

		public event OnGameConsole GameConsoleEvent
		{
			add
			{
				_GameConsoleEvent += value;
				if(_UserProvider != null)
				{
					value(_UserProvider);
				}
			}

			remove { _GameConsoleEvent -= value; }
		}

		private struct Provider
		{
			public IUserFactoty<TUser> Factory;

			public string Name;
		}

		private readonly Command _Command;

		private readonly List<Provider> _Providers;

		private readonly Console.IViewer _View;

		private UserProvider<TUser> _UserProvider;

		public GameModeSelector(Command command, Console.IViewer view)
		{
			_Command = command;
			_View = view;

			_Providers = new List<Provider>();
		}

		public void AddFactoty(string name, IUserFactoty<TUser> user_factory)
		{
			_Providers.Add(
				new Provider
				{
					Name = name, 
					Factory = user_factory
				});
			_View.WriteLine(string.Format("Added {0} factory.", name));
		}

		public UserProvider<TUser> CreateUserProvider(string name)
		{
			if(_UserProvider != null)
			{
				throw new SystemException("has user proivder!");
			}

			var factory = _Find(name);
			if(factory == null)
			{
				_View.WriteLine(string.Format("Game mode {0} not found.", name));
				return null;
			}

			_View.WriteLine(string.Format("CreateInstnace game console factory : {0}.", name));

			_UserProvider = new UserProvider<TUser>(factory, _View, _Command);
			if(_GameConsoleEvent != null)
			{
				_GameConsoleEvent(_UserProvider);
			}

			return _UserProvider;
		}

		private IUserFactoty<TUser> _Find(string name)
		{
			return (from provider in _Providers where provider.Name == name select provider.Factory).SingleOrDefault();
		}
	}
}
