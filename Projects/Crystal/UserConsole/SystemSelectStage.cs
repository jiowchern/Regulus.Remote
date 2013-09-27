using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UserConsole
{
    class SystemSelectStage : Regulus.Game.IStage<Framework>
    {
        public enum System
        { 
            Standalong,Remoting
        }

        Regulus.Game.StageLock Regulus.Game.IStage<Framework>.Enter(Framework obj)
        {
            obj.Command.Register("1", _OnRunStandalong);
            obj.Command.Register("2", _OnRunRemoting);
            obj.Viewer.WriteLine("選擇執行系統 1: [單機] 2:[連線]");            
            return null;
        }

        public event Action<System> RunSystemEvent;
        void _OnRunStandalong()
        {
            RunSystemEvent(System.Standalong);
        }
        void _OnRunRemoting()
        {
            RunSystemEvent(System.Remoting);
        }

        void Regulus.Game.IStage<Framework>.Leave(Framework obj)
        {
            obj.Command.Unregister("1");
            obj.Command.Unregister("2");
        }

        void Regulus.Game.IStage<Framework>.Update(Framework obj)
        {
            
        }
    }
}
