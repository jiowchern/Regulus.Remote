using System;
using System.Collections.Generic;

namespace Regulus.Remoting.Soul.Native
{    
	public class Application : Regulus.Utility.WindowConsole
	{
        Regulus.Game.StageMachine _Machine;
        protected override void _Launch()
        {
            _Machine = new Game.StageMachine();
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
            var stage = new Regulus.Remoting.Soul.Native.StageStart(base.Command, base.Viewer);
            stage.DoneEvent += _ToRun;
            _Machine.Push(stage);
        }

        private void _ToRun(Regulus.Game.ICore core, int port, float timeout)
        {
            var stage = new Regulus.Remoting.Soul.Native.StageRun(core, base.Command, port, base.Viewer);
            stage.ShutdownEvent += _ToStart;
            _Machine.Push(stage);
        }
    }
}