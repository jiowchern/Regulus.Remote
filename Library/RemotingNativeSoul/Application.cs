using Regulus.Utility;

namespace Regulus.Remoting.Soul.Native
{
	public class Application : WindowConsole
	{
		private StageMachine _Machine;

		private SpinWait _SpinWait;

		private TimeCounter _TimeCounter;

	    private readonly string[] _Args;

	    public Application(string[] args)
	    {
	        _Args = args;
	    }

        protected override void _Launch()
		{
			_SpinWait = new SpinWait();
			_TimeCounter = new TimeCounter();
			_Machine = new StageMachine();
			_ToStart();
		}

		protected override void _Update()
		{
			if(_TimeCounter.Second > 1.0f / 30.0f)
			{
				_Machine.Update();
				_TimeCounter.Reset();
				_SpinWait.Reset();
			}
			else
			{
				_SpinWait.SpinOnce();
			}
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
