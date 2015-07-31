// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Client.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the Client type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using Regulus.Utility;

#endregion

namespace Regulus.Framework
{
	public class Client<TUser> : IUpdatable
		where TUser : class, IUpdatable
	{
		public event OnModeSelector ModeSelectorEvent;

		private readonly Console _Console;

		private readonly StageMachine _Machine;

		private readonly Console.IViewer _View;

		private Command _Command
		{
			get { return this._Console.Command; }
		}

		public Command Command
		{
			get { return this._Command; }
		}

		public bool Enable { get; private set; }

		public GameModeSelector<TUser> Selector { get; private set; }

		public Client(Console.IViewer view, Console.IInput input)
		{
			this.Enable = true;
			this._Machine = new StageMachine();

			this._View = view;
			this._Console = new Console(input, view);
			this.Selector = new GameModeSelector<TUser>(this._Command, this._View);
		}

		bool IUpdatable.Update()
		{
			this._Machine.Update();
			return this.Enable;
		}

		void IBootable.Launch()
		{
			this._Command.Register("Quit", this._ToShutdown);
			this._ToSelectMode();
		}

		void IBootable.Shutdown()
		{
			this._ToShutdown();
		}

		public delegate void OnModeSelector(GameModeSelector<TUser> selector);

		private void _ToSelectMode()
		{
			var stage = new SelectMode<TUser>(this.Selector, this._Command);
			stage.DoneEvent += this._ToOnBoard;
			stage.InitialedEvent += () =>
			{
				if (this.ModeSelectorEvent != null)
				{
					this.ModeSelectorEvent(this.Selector);
				}
			};
			this._Machine.Push(stage);
		}

		private void _ToOnBoard(UserProvider<TUser> user_provider)
		{
			this._View.WriteLine("Onboard ready.");
			var stage = new OnBoard<TUser>(user_provider, this._Command);
			stage.DoneEvent += this._ToShutdown;
			this._Machine.Push(stage);
		}

		private void _ToShutdown()
		{
			this.Enable = false;
			this._Machine.Termination();
			this._Command.Unregister("Quit");
		}
	}


	namespace Extension
	{
		public static class ClientExtension
		{
			public static void Run(this IUpdatable updatable)
			{
			}
		}
	}
}