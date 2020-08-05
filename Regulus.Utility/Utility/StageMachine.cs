using Regulus.Utility;
using System;

namespace Regulus.Utility
{
    public class StageMachine : Utility.IBootable
    {
        Utility.IBootable _Current;

        public StageMachine()
        {
            _Current = this;
        }

        public void Push(Regulus.Utility.IBootable bootable)
        {
            _Current.Shutdown();
            bootable.Launch();
            _Current = bootable;
        }

        void IBootable.Launch()
        {
            
        }

        public void Clean()
        {
            Push(this);
        }

        void IBootable.Shutdown()
        {
            
        }
    }
}
