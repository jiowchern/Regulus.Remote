// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GameModeSelector.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the GameModeSelector type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System;
using System.Collections.Generic;
using System.Linq;

using Regulus.Utility;

using Console = Regulus.Utility.Console;

#endregion

namespace Regulus.Framework
{
	public class GameModeSelector<TUser>
		where TUser : class, IUpdatable
	{
		private event OnGameConsole _GameConsoleEvent;

		public event OnGameConsole GameConsoleEvent
		{
			add
			{
				this._GameConsoleEvent += value;
				if (this._UserProvider != null)
				{
					value(this._UserProvider);
				}
			}

			remove { this._GameConsoleEvent -= value; }
		}

		private readonly Command _Command;

		private readonly List<Provider> _Providers;

		private readonly Console.IViewer _View;

		private UserProvider<TUser> _UserProvider;

		public GameModeSelector(Command command, Console.IViewer view)
		{
			this._Command = command;
			this._View = view;

			this._Providers = new List<Provider>();
		}

		private struct Provider
		{
			public IUserFactoty<TUser> Factory;

			public string Name;
		}

		public delegate void OnGameConsole(UserProvider<TUser> console);

		public void AddFactoty(string name, IUserFactoty<TUser> user_factory)
		{
			this._Providers.Add(new Provider
			{
				Name = name, 
				Factory = user_factory
			});
			this._View.WriteLine(string.Format("Added {0} factory.", name));
		}

		public UserProvider<TUser> CreateUserProvider(string name)
		{
			if (this._UserProvider != null)
			{
				throw new SystemException("has user proivder!");
			}

			var factory = this._Find(name);
			if (factory == null)
			{
				this._View.WriteLine(string.Format("Game mode {0} not found.", name));
				return null;
			}

			this._View.WriteLine(string.Format("Create game console factory : {0}.", name));

			this._UserProvider = new UserProvider<TUser>(factory, this._View, this._Command);
			if (this._GameConsoleEvent != null)
			{
				this._GameConsoleEvent(this._UserProvider);
			}

			return this._UserProvider;
		}

		private IUserFactoty<TUser> _Find(string name)
		{
			return (from provider in this._Providers where provider.Name == name select provider.Factory).SingleOrDefault();
		}
	}
}