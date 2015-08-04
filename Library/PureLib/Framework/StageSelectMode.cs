// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StageSelectMode.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the SelectMode type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System;

using Regulus.Utility;

#endregion

namespace Regulus.Framework
{
	internal class SelectMode<TUser> : IStage
		where TUser : class, IUpdatable
	{
		public event OnDone<TUser> DoneEvent;

		public event Action InitialedEvent;

		private readonly Command _Command;

		private readonly GameModeSelector<TUser> _Selector;

		public SelectMode(GameModeSelector<TUser> mode_selector, Command command)
		{
			this._Command = command;
			this._Selector = mode_selector;
		}

		void IStage.Enter()
		{
			this._Selector.GameConsoleEvent += this._ObtainConsole;
			this._Command.Register<string>("CreateMode", this._CreateGameConsole);

			this.InitialedEvent();
		}

		void IStage.Leave()
		{
			this._Command.Unregister("CreateMode");
			this._Selector.GameConsoleEvent -= this._ObtainConsole;
		}

		void IStage.Update()
		{
		}

		public delegate void OnDone<TUser>(UserProvider<TUser> console) where TUser : class, IUpdatable;

		private void _ObtainConsole(UserProvider<TUser> console)
		{
			this.DoneEvent(console);
		}

		private void _CreateGameConsole(string name)
		{
			this._Selector.CreateUserProvider(name);
		}
	}
}