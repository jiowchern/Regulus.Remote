// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Application.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the Application type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using Regulus.Utility;

#endregion

namespace Regulus.Remoting.Soul.Native
{
	public class Application : WindowConsole
	{
		private StageMachine _Machine;

		private SpinWait _SpinWait;

		private TimeCounter _TimeCounter;

		protected override void _Launch()
		{
			this._SpinWait = new SpinWait();
			this._TimeCounter = new TimeCounter();
			this._Machine = new StageMachine();
			this._ToStart();
		}

		protected override void _Update()
		{
			if (this._TimeCounter.Second > 1.0f / 30.0f)
			{
				this._Machine.Update();
				this._TimeCounter.Reset();
				this._SpinWait.Reset();
			}
			else
			{
				this._SpinWait.SpinOnce();
			}
		}

		protected override void _Shutdown()
		{
			this._Machine.Termination();
		}

		private void _ToStart()
		{
			var stage = new StageStart(this.Command, this.Viewer);
			stage.DoneEvent += this._ToRun;
			this._Machine.Push(stage);
		}

		private void _ToRun(ICore core, int port, float timeout)
		{
			var stage = new StageRun(core, this.Command, port, this.Viewer);
			stage.ShutdownEvent += this._ToStart;
			this._Machine.Push(stage);
		}
	}
}