using Regulus.Framework;
using System;

namespace Regulus.Utility
{
    public class StageMachine : Framework.IBootable
    {
        Framework.IBootable _Current;

        public StageMachine()
        {
            _Current = this;
        }

        public void Push(Regulus.Framework.IBootable bootable)
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
