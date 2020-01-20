using Regulus.Utility;

namespace Regulus.Framework
{
	public class Client<TUser> : IUpdatable
		where TUser : class, IUpdatable
	{
		public delegate void OnModeSelector(GameModeSelector<TUser> selector);

		public event OnModeSelector ModeSelectorEvent;

		

		private readonly StatusMachine _Machine;

		private readonly Console.IViewer _View;

	    private Command _Command;
		

		public Command Command
		{
			get { return _Command; }
		}

		public bool Enable { get; private set; }

		public GameModeSelector<TUser> Selector { get; private set; }

	    public Client(Console.IViewer view , Command command)
	    {
            Enable = true;
            _Machine = new StatusMachine();

            _View = view;
	        _Command = command;
            Selector = new GameModeSelector<TUser>(_Command, _View);
        }
        

		bool IUpdatable.Update()
		{
			_Machine.Update();
			return Enable;
		}

		void IBootable.Launch()
		{
			_Command.Register("Quit", _ToShutdown);
			_ToSelectMode();
		}

		void IBootable.Shutdown()
		{
			_ToShutdown();
		}

		private void _ToSelectMode()
		{
			var stage = new SelectMode<TUser>(Selector, _Command);
			stage.DoneEvent += _ToOnBoard;
			stage.InitialedEvent += () =>
			{
				if(ModeSelectorEvent != null)
				{
					ModeSelectorEvent(Selector);
				}
			};
			_Machine.Push(stage);
		}

		private void _ToOnBoard(UserProvider<TUser> user_provider)
		{
			_View.WriteLine("Onboard ready.");
			var stage = new OnBoard<TUser>(user_provider, _Command);
			stage.DoneEvent += _ToShutdown;
			_Machine.Push(stage);
		}

		private void _ToShutdown()
		{
			Enable = false;
			_Machine.Termination();
			_Command.Unregister("Quit");
		}
	}

	
}
