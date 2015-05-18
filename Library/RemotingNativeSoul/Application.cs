using System;
using System.Collections.Generic;

namespace Regulus.Remoting.Soul.Native
{    
	public class Application : Regulus.Utility.WindowConsole
	{
        System.Threading.SpinWait _SpinWait;
        Regulus.Utility.TimeCounter _TimeCounter;
        Regulus.Utility.StageMachine _Machine;
        protected override void _Launch()
        {
            _SpinWait = new System.Threading.SpinWait();
            _TimeCounter = new Utility.TimeCounter();
            _Machine = new Utility.StageMachine();
            _ToStart();        
        }

        protected override void _Update()
        {
            
            if (_TimeCounter.Second > 1.0f / 30.0f)
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
            var stage = new Regulus.Remoting.Soul.Native.StageStart(base.Command, base.Viewer);
            stage.DoneEvent += _ToRun;
            _Machine.Push(stage);
        }

        private void _ToRun(Regulus.Utility.ICore core, int port, float timeout)
        {
            var stage = new Regulus.Remoting.Soul.Native.StageRun(core, base.Command, port, base.Viewer);
            stage.ShutdownEvent += _ToStart;
            _Machine.Push(stage);
        }
    }
}