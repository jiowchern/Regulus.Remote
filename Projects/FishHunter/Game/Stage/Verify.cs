using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGame.Project.FishHunter.Stage
{
    class Verify : Regulus.Utility.IStage
    {
        VGame.Project.FishHunter.Verify _Verify;
        Regulus.Remoting.ISoulBinder _Binder;

        public event VGame.Project.FishHunter.Verify.DoneCallback DoneEvent;
        public Verify(Regulus.Remoting.ISoulBinder binder , VGame.Project.FishHunter.Verify verify)
        {
            _Verify = verify;
            _Binder = binder;
        }
        void Regulus.Utility.IStage.Enter()
        {
            _Verify.DoneEvent += DoneEvent;

            _Binder.Bind<VGame.Project.FishHunter.IVerify>(_Verify);
        }

        
        void Regulus.Utility.IStage.Leave()
        {
            _Binder.Unbind<VGame.Project.FishHunter.IVerify>(_Verify);
            _Verify.DoneEvent -= DoneEvent;
        }

        void Regulus.Utility.IStage.Update()
        {
            
        }
    }
}
