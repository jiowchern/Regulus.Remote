using System;
using Regulus.Network.RUDP;

namespace Regulus.Network.Tests.HostApp
{
	internal class HostConsole : Regulus.Utility.WindowConsole
	{
		Regulus.Utility.StageMachine _Machine;
		public HostConsole()
		{
			_Machine = new Utility.StageMachine();
		}

		protected override void _Launch()
		{
			_ToInitial();
		}

		private void _ToInitial()
		{
			var stage = new InitialStage(Command);
			stage.CreatedEvent += _ToRun;
			_Machine.Push(stage);
		}

		private void _ToRun(Host host)
		{
			var stage = new RunStage(Command , Viewer,  host );
			stage.ExitEvent += _ToInitial;
			_Machine.Push(stage);
		}

		protected override void _Shutdown()
		{
			_Machine.Termination();
		}

		protected override void _Update()
		{
			_Machine.Update();
		}
	}
}