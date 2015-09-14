using Regulus.Utility;

namespace Regulus.Remoting.Soul.Native
{
	public class Application : WindowConsole
	{
		private StageMachine _Machine;

        private readonly string[] _Args;

	    public Application(string[] args)
	    {
	        _Args = args;
	    }

        protected override void _Launch()
		{
            _Machine = new StageMachine();
			_ToStart();
		}

		protected override void _Update()
		{			            
            _Machine.Update();			
		}

		protected override void _Shutdown()
		{
			_Machine.Termination();
		}

		private void _ToStart()
		{
			var stage = new StageStart(Command, Viewer , _Args);
			stage.DoneEvent += _ToRun;
			_Machine.Push(stage);
		}

		private void _ToRun(ICore core, int port, float timeout)
		{
			var stage = new StageRun(core, Command, port, Viewer);
			stage.ShutdownEvent += _ToStart;
			_Machine.Push(stage);
		}
	}
}
