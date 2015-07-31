// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StageOnboard.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the OnBoard type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using Regulus.Utility;

#endregion

namespace Regulus.Framework
{
	internal class OnBoard<TUser> : IStage
		where TUser : class, IUpdatable
	{
		public event OnDone DoneEvent;

		private readonly Command _Command;

		private readonly Updater _Updater;

		private readonly UserProvider<TUser> _UserProvider;

		public OnBoard(UserProvider<TUser> user_provider, Command command)
		{
			this._Updater = new Updater();
			this._UserProvider = user_provider;
			this._Command = command;
		}

		void IStage.Enter()
		{
			this._Updater.Add(this._UserProvider);
			this._Command.Register<string>("SpawnUser[UserName]", this._Spawn);
			this._Command.Register<string>("UnpawnUser[UserName]", this._Unspawn);
			this._Command.Register<string>("SelectUser[UserName]", this._Select);
		}

		void IStage.Leave()
		{
			this._Command.Unregister("SelectUser");
			this._Command.Unregister("SpawnUser");
			this._Command.Unregister("UnpawnUser");
			this._Updater.Shutdown();
		}

		void IStage.Update()
		{
			this._Updater.Working();
		}

		public delegate void OnDone();

		private void _Spawn(string name)
		{
			this._UserProvider.Spawn(name);
		}

		private void _Unspawn(string name)
		{
			this._UserProvider.Unspawn(name);
		}

		private void _Select(string name)
		{
			this._UserProvider.Select(name);
		}
	}
}