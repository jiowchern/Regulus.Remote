using Regulus.Utiliey;
using System;

namespace Regulus.Utility
{
    public class StageMachine : Utiliey.IBootable
    {
        Utiliey.IBootable _Current;

        public StageMachine()
        {
            _Current = this;
        }

        public void Push(Regulus.Utiliey.IBootable bootable)
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
